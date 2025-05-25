using Nonuso.Domain.Entities;

namespace Nonuso.Domain.IRepos
{
    public interface IFavoriteRepository
    {
        Task<Favorite?> GetByUserAndProductIdAsync(Guid userId, Guid productId);
        Task<IEnumerable<Favorite>> GetByUserIdAsync(Guid id);
        Task<Favorite?> GetByIdAsync(Guid id);
        Task CreateAsync(Favorite entity);
        Task DeleteAsync(Favorite entity);
    }
}
