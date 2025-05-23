using Microsoft.EntityFrameworkCore;
using Nonuso.Domain.Entities;
using Nonuso.Domain.Entities.Base;
using Nonuso.Domain.IRepos;

namespace Nonuso.Infrastructure.Persistence.Repos
{
    internal class ProductRequestRepository(NonusoDbContext context) : IProductRequestRepository
    {
        private readonly NonusoDbContext _context = context;

        public async Task<IEnumerable<ProductRequest>> GetByProductIdAsync(Guid productId, Guid? userId = null)
        {
            return await _context.ProductRequest
                .Where(x => x.ProductId == productId && (userId == null || x.RequestedId == userId))
                .ToListAsync();
        }

        public async Task CreateAsync(ProductRequest entity)
        {
            _context.ProductRequest.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductRequest entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _context.ProductRequest.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<ProductRequest> entities)
        {
            _context.ProductRequest.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }
    }
}
