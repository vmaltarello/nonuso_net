using Nonuso.Domain.Entities;
using Nonuso.Domain.Models;

namespace Nonuso.Domain.IRepos
{
    public interface IAuthRepository
    {
        Task<UserProfileModel> GetUserProfileAsync(Guid id);
        Task<RefreshToken?> GetRefreshTokenByUserIdAsync(Guid id, string? refreshToken = null);
        Task CreateRefreshTokenAsync(RefreshToken entity);
        Task RevokeRefreshTokenAsync(RefreshToken entity);
        Task DeleteAsync(User entity);
    }
}
