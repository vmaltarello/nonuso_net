using Nonuso.Domain.Entities;
using Nonuso.Messages.Api;

namespace Nonuso.Application.IServices
{
    public interface IAuthService
    {
        Task<UserResultModel> AuthWithGoogleAsync(string idToken);
        Task SignUpAsync(UserSignUpParamModel model);
        Task<UserResultModel?> SignInAsync(UserSignInParamModel model);
        Task SignOutAsync(Guid id);
        Task ChangePasswordAsync(UserChangePasswordModel model);
        Task ChangeUserNameAsync(Guid userId, string userName);
        Task<UserResultModel> RefreshTokenAsync(Guid userId, string refreshToken);
        Task<bool> UserNameIsUniqueAsync(string userName);
        Task ConfirmEmailAsync(string token, string email);
    }
}
