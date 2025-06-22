using Nonuso.Messages.Api;

namespace Nonuso.Application.IServices
{
    public interface IAuthService
    {
        Task<UserResultModel> GetCurrentUserAsync(Guid id);
        Task<UserProfileResultModel> GetUserProfileAsync(Guid id);
        Task<UserResultModel> AuthWithGoogleAsync(string idToken);
        Task SignUpAsync(UserSignUpParamModel model);
        Task<UserResultModel> SignInAsync(UserSignInParamModel model);
        Task SignOutAsync(Guid id);
        Task DeleteAsync(Guid id);
        Task ChangePasswordAsync(UserChangePasswordParamModel model, Guid userId);
        Task ChangeUserNameAsync(Guid userId, string userName);
        Task<UserResultModel> RefreshTokenAsync(RefreshTokenParamModel model);
        Task<UserResultModel?> ConfirmEmailAsync(AuthConfirmEmailParamModel model);
        Task RequestResetPasswordAsync(RequestResetPasswordParamModel model);
        Task<UserResultModel?> ResetPasswordAsync(ResetPasswordParamModel model);
    }
}
