using Nonuso.Application.IServices;
using Nonuso.Common;
using Nonuso.Domain.IRepos;
using Nonuso.Messages.Api;

namespace Nonuso.Application.Services
{
    internal class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
    {
        readonly ICategoryRepository _categoryRepository = categoryRepository;

        public async Task<IEnumerable<CategoryResultModel>> GetAllAsync()
        {
            var result = await _categoryRepository.GetAllAsync();

            return result.To<CategoryResultModel[]>();
        }

        public async Task<IEnumerable<CategoryResultModel>> GetAllPopularAsync(Guid? userId = null)
        {
            var result = await _categoryRepository.GetAllPopularAsync(userId);

            return result.To<CategoryResultModel[]>();
        }
    }
}
