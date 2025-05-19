using Nonuso.Messages.Api;

namespace Nonuso.Application.IServices
{
    public interface IAuthService
    {
        Task<UserResultModel> GetCurrentUserAsync(Guid id);
        Task<UserResultModel> AuthWithGoogleAsync(string idToken);
        Task SignUpAsync(UserSignUpParamModel model);
        Task<UserResultModel?> SignInAsync(UserSignInParamModel model);
        Task SignOutAsync(Guid id);
        Task DeleteAsync(Guid id);
        Task ChangePasswordAsync(string password, Guid userId);
        Task ChangeUserNameAsync(Guid userId, string userName);
        Task<UserResultModel> RefreshTokenAsync(RefreshTokenParamModel model);
        Task<bool> UserNameIsUniqueAsync(string userName);
        Task ConfirmEmailAsync(string token, string email);
    }
}
