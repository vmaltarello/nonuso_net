using Nonuso.Messages.Api;

namespace Nonuso.Application.IServices
{
    public interface IFavoriteService
    {
        Task<IEnumerable<FavoriteResultModel>> GetByUserIdAsync(Guid id);
        Task CreateAsync(Guid userId, Guid productId);
        Task DeleteAsync(Guid id);
    }
}
