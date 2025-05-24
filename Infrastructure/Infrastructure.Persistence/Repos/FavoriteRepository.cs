using Microsoft.EntityFrameworkCore;
using Nonuso.Domain.Entities;
using Nonuso.Domain.IRepos;

namespace Nonuso.Infrastructure.Persistence.Repos
{
    internal class FavoriteRepository(NonusoDbContext context) : IFavoriteRepository
    {
        private readonly NonusoDbContext _context = context;

        public async Task<IEnumerable<Favorite>> GetByUserIdAsync(Guid id)
        {
            return await _context.Favorite
                            .Where(x => x.UserId == id)                            
                            .Include(x => x.Product).ThenInclude(x => x!.User)
                            .ToListAsync();
        }

        public async Task<Favorite?> GetByIdAsync(Guid id)
        {
            return await _context.Favorite
                            .Where(x => x.Id == id)
                            .Include(x => x.User)
                            .Include(x => x.Product)
                            .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Favorite entity)
        {
            _context.Favorite.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Favorite entity)
        {
            _context.Favorite.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
