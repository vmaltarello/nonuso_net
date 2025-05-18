using Nonuso.Domain.Entities;

namespace Nonuso.Domain.IRepos
{
    public interface IAuthRepository
    {
        Task<RefreshToken?> GetRefreshTokenByUserIdAsync(Guid id, string? refreshToken = null);
        Task CreateRefreshTokenAsync(RefreshToken entity);
        Task RevokeRefreshTokenAsync(RefreshToken entity);
        Task DeleteAsync(User entity);
    }
}
