using Microsoft.EntityFrameworkCore;
using Nonuso.Domain.Entities;
using Nonuso.Domain.IRepos;

namespace Nonuso.Infrastructure.Persistence.Repos
{
    internal class CategoryRepository(NonusoDbContext context) : ICategoryRepository
    {
        private readonly NonusoDbContext _context = context;

        public async Task<IEnumerable<Category>> GetAllAsync() => await _context.Category.OrderBy(x => x.Description).ToListAsync();

        public async Task<IEnumerable<Category?>> GetAllPopularAsync(Guid? userId = null)
        {
            return await _context.Product
                  .Where(x => userId == null || x.UserId != userId)
                  .GroupBy(x => x.CategoryId)
                  .OrderBy(x => x.Count())
                  .Select(x => x.First().Category)
                  .Take(5)
                  .ToListAsync();
        }
    }
}