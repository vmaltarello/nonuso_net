using Nonuso.Domain.Entities;
using Nonuso.Domain.Models;

namespace Nonuso.Domain.IRepos
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id);
        Task<ProductDetailModel?> GetDetailsAsync(Guid id, Guid userId);
        Task<IEnumerable<Product>> GetAllPopularAsync(Guid? userId = null);
        Task CreateAsync(Product entity);
        Task UpdateAsync(Product entity);
        Task DeleteAsync(Product entity);
    }
}
