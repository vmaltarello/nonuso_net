using Nonuso.Domain.Entities;

namespace Nonuso.Domain.IRepos
{
    public interface IProductRequestRepository
    {
        Task<IEnumerable<ProductRequest>> GetByProductIdAsync(Guid productId, Guid? userId = null);
        Task CreateAsync(ProductRequest entity);
        Task UpdateAsync(ProductRequest entity);
        Task UpdateRangeAsync(IEnumerable<ProductRequest> entities);

    }
}
