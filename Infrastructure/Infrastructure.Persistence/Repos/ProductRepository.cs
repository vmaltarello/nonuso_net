using Nonuso.Domain.Entities;
using Nonuso.Domain.IRepos;

namespace Nonuso.Infrastructure.Persistence.Repos
{
    internal class ProductRepository(NonusoDbContext context) : IProductRepository
    {
        private readonly NonusoDbContext _context = context;

        public async Task CreateAsync(Product entity)
        {
            _context.Product.Add(entity);
            await _context.SaveChangesAsync();
        }

    }
}
