using Nonuso.Messages.Api;

namespace Nonuso.Application.IServices
{
    public interface IProductService
    {
        Task<ProductDetailResultModel> GetDetailsAsync(Guid id, Guid userId);
        Task<IEnumerable<ProductResultModel>> GetAllPopularAsync(Guid? userId = null);
        Task CreateAsync(ProductParamModel model);
        Task DeleteAsync(Guid id);
    }
}
