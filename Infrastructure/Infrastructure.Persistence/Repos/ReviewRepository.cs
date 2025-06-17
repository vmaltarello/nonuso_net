using Nonuso.Domain.Entities;
using Nonuso.Domain.IRepos;

namespace Nonuso.Infrastructure.Persistence.Repos
{
    internal class ReviewRepository(NonusoDbContext context) : IReviewRepository
    {
        private readonly NonusoDbContext _context = context;

        public async Task CreateAsync(Review entity)
        {
            _context.Review.Add(entity);
            await _context.SaveChangesAsync();
        }
    }
}
