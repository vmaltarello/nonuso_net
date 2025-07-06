using AutoMapper;
using FluentValidation;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Nonuso.Application.IServices;
using Nonuso.Common;
using Nonuso.Domain.Entities;
using Nonuso.Domain.Exceptions;
using Nonuso.Domain.IRepos;
using Nonuso.Domain.Validators.Factory;
using Nonuso.Infrastructure.Secret;
using Nonuso.Messages.Api;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Nonuso.Infrastructure.Auth.Services
{
    public class AuthService(
        IMapper mapper,
        ILogger<AuthService> logger,
        IDomainValidatorFactory validatorFactory,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IAuthRepository authRepository,
        INotificationService oneSignalService,
        IConfiguration configuration,
        ISecretManager secretManager) : IAuthService
    {
        readonly IMapper _mapper = mapper;
        readonly ILogger<AuthService> _logger = logger;
        readonly IDomainValidatorFactory _validatorFactory = validatorFactory;
        readonly UserManager<User> _userManager = userManager;
        readonly SignInManager<User> _signInManager = signInManager;
        readonly IAuthRepository _authRepository = authRepository;
        readonly INotificationService _oneSignalService = oneSignalService;
        readonly IConfiguration _configuration = configuration;
        readonly ISecretManager _secretManager = secretManager;
        readonly string _wrongCredentialMessage = "wrongCredential";

        public async Task<UserResultModel> GetCurrentUserAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString())
               ?? throw new EntityNotFoundException(nameof(User), id);

            return user.To<UserResultModel>();
        }

        public async Task<UserProfileResultModel> GetUserProfileAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString())
               ?? throw new EntityNotFoundException(nameof(User), id);

            var profileInfo = await _authRepository.GetUserProfileAsync(user.Id);

            var result = new UserProfileResultModel()
            {
                UserName = user.UserName!,
                Reviews = _mapper.Map<ReviewResultModel[]>(profileInfo.Reviews),
                JoinedMonth = profileInfo.JoinedMonth,
                JoinedYear = profileInfo.JoinedYear,
                ProductCount = profileInfo.ProductCount
            };

            return result;
        }

        public async Task<UserResultModel> AuthWithGoogleAsync(string idToken)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);

            var email = payload.Email;
            var googleId = payload.Subject;

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                var subEmailPart = email.Split('@')[0];
                user = new User
                {
                    UserName = subEmailPart[..Math.Min(20, subEmailPart.Length)].ToLower(),
                    Email = email,
                    EmailConfirmed = true,
                    LastSignInAt = DateTime.UtcNow
                };

                await _userManager.CreateAsync(user);

                var createdUser = await _userManager.FindByEmailAsync(email);

                var loginInfo = new UserLoginInfo("Google", googleId, "Google");
                var loginResult = await _userManager.AddLoginAsync(user, loginInfo);

                return await BuildTokens(createdUser!);
            }

            var existingLogins = await _userManager.GetLoginsAsync(user);

            if (!existingLogins.Any(x => x.LoginProvider == "Google" && x.ProviderKey == googleId))
            {
                var loginInfo = new UserLoginInfo("Google", googleId, "Google");
                var loginResult = await _userManager.AddLoginAsync(user, loginInfo);
            }

            return await BuildTokens(user);
        }

        public async Task SignUpAsync(UserSignUpParamModel model)
        {
            var entity = model.To<User>();
            entity.UserName = entity.UserName?.ToLower().Trim();

            _validatorFactory.GetValidator<User>().ValidateAndThrow(entity);

            var result = await _userManager.CreateAsync(entity, model.Password);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var encodedToken = WebUtility.UrlEncode(token);
                    var link = $"nonuso.app://confirmEmail?token={encodedToken}&email={model.Email}";
                    await _oneSignalService.SendConfirmEmailAsync(user, link);
                }
            }
        }

        public async Task<UserResultModel> SignInAsync(UserSignInParamModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email) ??
                throw new AuthWrongCredentialException(_wrongCredentialMessage);

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);

            if (result.Succeeded)
            {
                if (!user.EmailConfirmed || !user.IsEnabled) throw new AuthWrongCredentialException(_wrongCredentialMessage);

                user.LastSignInAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                return await BuildTokens(user);
            }

            throw new AuthWrongCredentialException(_wrongCredentialMessage);
        }

        public async Task SignOutAsync(Guid id)
        {
            await _signInManager.SignOutAsync();

            var refreshToken = await _authRepository.GetRefreshTokenByUserIdAsync(id);

            if (refreshToken != null)
                await _authRepository.RevokeRefreshTokenAsync(refreshToken);
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString())
                ?? throw new EntityNotFoundException(nameof(User), id);

            await _authRepository.DeleteAsync(user);
        }

        public async Task ChangePasswordAsync(UserChangePasswordParamModel model, Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new EntityNotFoundException(nameof(User), userId);

            await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        }

        public async Task ChangeUserNameAsync(UserChangeUserNameParamModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString())
                ?? throw new EntityNotFoundException(nameof(User), model.UserId);

            user.UserName = model.UserName.ToLower().Trim();

            await _userManager.UpdateAsync(user);
        }

        public async Task<UserResultModel> RefreshTokenAsync(RefreshTokenParamModel model)
        {
            var storedToken = await _authRepository.GetRefreshTokenByUserIdAsync(model.UserId, model.RefreshToken);

            if (storedToken == null || storedToken.ExpirationDate < DateTime.UtcNow)
                throw new AuthUnauthorizedException();

            await _authRepository.RevokeRefreshTokenAsync(storedToken);

            return await BuildTokens(storedToken.User!);
        }

        public async Task<UserResultModel?> ConfirmEmailAsync(AuthConfirmEmailParamModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email)
                ?? throw new EntityNotFoundException(nameof(User), model.Email);

            var result = await _userManager.ConfirmEmailAsync(user, model.Token);

            if (result.Succeeded)
                return await BuildTokens(user);

            return null;
        }

        public async Task<UserResultModel?> ResetPasswordAsync(ResetPasswordParamModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email)
                ?? throw new AuthUnauthorizedException();

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);

            if (result.Succeeded) return await BuildTokens(user);

            return null;
        }

        public async Task RequestResetPasswordAsync(RequestResetPasswordParamModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null && user.EmailConfirmed && user.IsEnabled)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebUtility.UrlEncode(token);
                var link = $"nonuso.app://resetPassword?token={encodedToken}&email={model.Email}";
                await _oneSignalService.SendRequestResetPasswordEmailAsync(user, link);
            }
        }

        #region PRIVATE

        private async Task<UserResultModel> BuildTokens(User user)
        {
            var token = await GenerateJwtTokenAsync(user);
            var refreshToken = GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken()
            {
                Token = refreshToken,
                ExpirationDate = DateTime.UtcNow.AddDays(30),
                UserId = user.Id,
            };

            await _authRepository.CreateRefreshTokenAsync(refreshTokenEntity);

            return new UserResultModel()
            {
                Id = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                AccessToken = token,
                RefreshToken = refreshToken,
                RefreshTokenExpiresIn = 2592000 // 30 days
            };
        }

        private async Task<string> GenerateJwtTokenAsync(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                // this guarantees the token is unique
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretManager.GetJwtSecret()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddSeconds(double.Parse(_configuration["Jwt:Expires"]!)),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }

        #endregion
    }
}
