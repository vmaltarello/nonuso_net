using Microsoft.EntityFrameworkCore;
using Nonuso.Domain.Entities;
using Nonuso.Domain.IRepos;

namespace Nonuso.Infrastructure.Persistence.Repos
{
    internal class ReviewRepository(NonusoDbContext context) : IReviewRepository
    {
        private readonly NonusoDbContext _context = context;

        public async Task<IEnumerable<Review>> GetAllAsync(Guid userId)
        {
            return await _context.Review.Where(x => x.ReviewedUserId == userId).ToListAsync();
        }

        public async Task CreateAsync(Review entity)
        {
            _context.Review.Add(entity);
            await _context.SaveChangesAsync();
        }
    }
}
