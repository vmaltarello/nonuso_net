using Nonuso.Common.Filters;
using Nonuso.Messages.Api;

namespace Nonuso.Application.IServices
{
    public interface IProductService
    {
        Task<ProductDetailResultModel> GetByIdAsync(Guid id, Guid userId);
        Task<IEnumerable<ProductResultModel>> GetAllPopularAsync(Guid? userId = null);
        Task<IEnumerable<ProductResultModel>> GetAllActiveAsync(Guid userId);
        Task<IEnumerable<ProductResultModel>> SearchAsync(ProductFilter filters);
        Task CreateAsync(ProductParamModel model);
        Task UpdateAsync();
        Task DeleteAsync(Guid id);
    }
}
