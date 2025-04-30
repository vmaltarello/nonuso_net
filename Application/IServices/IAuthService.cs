using Nonuso.Messages.Api;

namespace Nonuso.Application.IServices
{
    public interface IAuthService
    {
        Task SignUpAsync(UserSignUpParamModel model);
        Task<UserResultModel?> SignInAsync(UserSignInParamModel model);
        Task SignOutAsync(Guid id);
        Task<UserResultModel> RefreshTokenAsync(Guid userId, string refreshToken);
        Task<bool> UserNameIsUniqueAsync(string userName);
    }
}
