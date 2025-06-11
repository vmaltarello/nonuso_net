using Nonuso.Common.Filters;
using Nonuso.Domain.Entities;
using Nonuso.Domain.Models;

namespace Nonuso.Domain.IRepos
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id);
        Task<ProductDetailModel?> GetByIdAsync(Guid id, Guid userId);
        Task<IEnumerable<Product>> GetAllPopularAsync(Guid? userId = null);
        Task<IEnumerable<Product>> GetAllActiveAsync(Guid userId);
        Task<IEnumerable<Product>> SearchAsync(ProductFilter filters);
        Task CreateAsync(Product entity);
        Task UpdateAsync(Product entity);
        Task DeleteAsync(Product entity);
        Task Report(ProductReport entity);
    }
}
