using Nonuso.Domain.Entities;

namespace Nonuso.Domain.IRepos
{
    public interface IProductRequestRepository
    {
        Task CreateAsync(ProductRequest entity);
        Task<ProductRequest?> GetActiveAsync(Guid userId, Guid productId);
    }
}
