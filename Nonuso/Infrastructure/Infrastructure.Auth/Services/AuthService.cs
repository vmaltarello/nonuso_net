using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Nonuso.Application.IServices;
using Nonuso.Common;
using Nonuso.Domain.Entities;
using Nonuso.Messages.Api;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Nonuso.Infrastructure.Auth.Services
{
    public class AuthService(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config) : IAuthService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;

        private readonly IConfiguration _config = config;

        public async Task SignUpAsync(UserSignUpParamModel model)
        {
            //var user = new AppUser
            //{
            //    Email = request.Email,
            //    UserName = request.Email,
            //    UserNameCustom = request.UserName,
            //    ProfileImageUrl = request.ProfileImageUrl
            //};

            var entity = model.To<User>();
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;

            var result = await _userManager.CreateAsync(model.To<User>(), model.Password);

            //if (!result.Succeeded)
            //    throw new ApplicationException("Registration failed");

            //return await GenerateJwtTokenAsync(user);
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

                    return new UserResultModel()
                    {
                        Id = user.Id,
                        Email = user.Email!,
                        UserName = user.UserName!,
                        Token = token,
                    };
                }               
            }

            throw new Exception("Errore login");            
        }

        public async Task RefreshTokenAsync(string refreshToken)
        {
            // OPTIONAL: Qui puoi implementare la logica per gestire refresh token
            throw new NotImplementedException("Refresh token not implemented yet");
        }

        private async Task<string> GenerateJwtTokenAsync(User user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                // this guarantees the token is unique
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new("UserName", user.UserName ?? string.Empty)
            };

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddSeconds(double.Parse(_config["Jwt:Expires"]!)),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
