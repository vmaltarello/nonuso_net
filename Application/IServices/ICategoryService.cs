using Nonuso.Messages.Api;

namespace Nonuso.Application.IServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResultModel>> GetAllAsync();
        Task<IEnumerable<CategoryResultModel>> GetAllPopularAsync(Guid? userId = null);
    }
}
