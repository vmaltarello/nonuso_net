using Nonuso.Domain.Entities;

namespace Nonuso.Domain.IRepos
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<IEnumerable<Category?>> GetAllPopularAsync(Guid? userId = null);
    }
}
