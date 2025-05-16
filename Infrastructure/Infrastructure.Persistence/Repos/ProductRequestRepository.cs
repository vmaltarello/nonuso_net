using Microsoft.EntityFrameworkCore;
using Nonuso.Domain.Entities;
using Nonuso.Domain.IRepos;

namespace Nonuso.Infrastructure.Persistence.Repos
{
    internal class ProductRequestRepository(NonusoDbContext context) : IProductRequestRepository
    {
        private readonly NonusoDbContext _context = context;

        public async Task CreateAsync(ProductRequest entity)
        {
            _context.ProductRequest.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<ProductRequest?> GetActiveAsync(Guid userId, Guid productId)
        {
            return await _context.ProductRequest.Where(x => x.Status == ProductRequestStatus.Pending
                                               && x.RequesterId == userId
                                               && x.ProductId == productId)
                                    .FirstOrDefaultAsync();
        }
    }
}
