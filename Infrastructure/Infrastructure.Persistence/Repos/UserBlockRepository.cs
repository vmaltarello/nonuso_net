using Microsoft.EntityFrameworkCore;
using Nonuso.Domain.Entities;
using Nonuso.Domain.IRepos;

namespace Nonuso.Infrastructure.Persistence.Repos
{
    internal class UserBlockRepository(NonusoDbContext context) : IUserBlockRepository
    {
        private readonly NonusoDbContext _context = context;

        public async Task<UserBlock?> GetByIdAsync(Guid id)
        {
            return await _context.UserBlock.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task CreateAsync(UserBlock entity)
        {
            _context.UserBlock.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(UserBlock entity)
        {
            _context.UserBlock.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
