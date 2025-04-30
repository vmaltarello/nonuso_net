using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Nonuso.Application.IServices;
using Nonuso.Common;
using Nonuso.Domain.Entities;
using Nonuso.Domain.Exceptions;
using Nonuso.Domain.IRepos;
using Nonuso.Messages.Api;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Nonuso.Infrastructure.Auth.Services
{
    public class AuthService(
        UserManager<User> userManager, 
        SignInManager<User> signInManager,
        IAuthRepository authRepository,
        IConfiguration configuration) : IAuthService
    {
        readonly UserManager<User> _userManager = userManager;
        readonly SignInManager<User> _signInManager = signInManager;
        readonly IAuthRepository _authRepository = authRepository;

        readonly IConfiguration _configuration = configuration;

        public async Task SignUpAsync(UserSignUpParamModel model)
        {
            var entity = model.To<User>();

            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;

            await _userManager.CreateAsync(entity, model.Password);
        }

        public async Task<UserResultModel?> SignInAsync(UserSignInParamModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, true, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user != null)
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
            }

            return null;    
        }

        public async Task SignOutAsync(Guid id)
        {
            await _signInManager.SignOutAsync();

            var refreshToken = await _authRepository.GetRefreshTokenByUserIdAsync(id)
                ?? throw new EntityNotFoundException(nameof(RefreshToken));

            await _authRepository.RevokeRefreshTokenAsync(refreshToken);
        }

        public async Task<UserResultModel> RefreshTokenAsync(Guid userId, string refreshToken)
        {
            var storedToken = await _authRepository.GetRefreshTokenByUserIdAsync(userId, refreshToken);

            if (storedToken == null || storedToken.ExpirationDate < DateTime.UtcNow)
                throw new UnauthorizedAccessException();

            var newAccessToken = await GenerateJwtTokenAsync(storedToken.User!);
            var newRefreshToken = GenerateRefreshToken();

            await _authRepository.RevokeRefreshTokenAsync(storedToken);

            var newTokenEntity = new RefreshToken
            {
                UserId = storedToken.UserId,
                Token = newRefreshToken,
                CreatedAt = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };

            await _authRepository.CreateRefreshTokenAsync(newTokenEntity);

            return new UserResultModel()
            {
                Id = storedToken.User!.Id,
                Email = storedToken.User!.Email!,
                UserName = storedToken.User!.UserName!,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiresIn = 2592000 // 30 days
            };
        }

        public async Task<bool> UserNameIsUniqueAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName.Trim()) == null;
        }
        
        #region PRIVATE

        private async Task<string> GenerateJwtTokenAsync(User user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),                
                new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new(JwtRegisteredClaimNames.Nickname, user.UserName ?? string.Empty),
                // this guarantees the token is unique
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
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
