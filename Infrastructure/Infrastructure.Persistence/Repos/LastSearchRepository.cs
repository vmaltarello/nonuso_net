using Microsoft.EntityFrameworkCore;
using Nonuso.Domain.Entities;
using Nonuso.Domain.IRepos;

namespace Nonuso.Infrastructure.Persistence.Repos
{
    internal class LastSearchRepository(NonusoDbContext context) : ILastSearchRepository
    {
        private readonly NonusoDbContext _context = context;

        public async Task CreateAsync(LastSearch entity)
        {
            _context.LastSearch.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<string>> GetByUserId(Guid id)
        {
            return await _context.LastSearch.Where(x => x.UserId == id).Take(3).Select(x => x.Search).ToListAsync();
        }

    }
}
