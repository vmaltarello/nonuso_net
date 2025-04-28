using Nonuso.Messages.Api;

namespace Nonuso.Application.IServices
{
    public interface IAuthService
    {
        Task SignUpAsync(UserSignUpParamModel model);
        Task<UserResultModel?> SignInAsync(UserSignInParamModel model);
        Task RefreshTokenAsync(string refreshToken);
    }
}
