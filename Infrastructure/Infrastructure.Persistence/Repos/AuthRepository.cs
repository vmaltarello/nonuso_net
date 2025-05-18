using Microsoft.EntityFrameworkCore;
using Nonuso.Domain.Entities;
using Nonuso.Domain.IRepos;

namespace Nonuso.Infrastructure.Persistence.Repos
{
    internal class AuthRepository(NonusoDbContext context) : IAuthRepository
    {
        private readonly NonusoDbContext _context = context;

        public async Task<RefreshToken?> GetRefreshTokenByUserIdAsync(Guid id, string? refreshToken = null)
        {
            return await _context.RefreshToken
                .Where(x => x.UserId == id 
                            && (refreshToken == null || x.Token == refreshToken)
                            && !x.Revoked)
                .Include(x => x.User)
                .FirstOrDefaultAsync();
        }

        public async Task CreateRefreshTokenAsync(RefreshToken entity)
        {
            _context.RefreshToken.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RevokeRefreshTokenAsync(RefreshToken entity)
        {
            entity.Revoked = true;

            _context.RefreshToken.Update(entity);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User entity)
        {
            entity.IsEnabled = false;           
            entity.DeletedAt = DateTime.UtcNow;
            entity.UserName = entity.UserName + "_deleted";
            entity.Email = entity.Email + "_deleted";

            _context.Users.Update(entity);
            
            await _context.SaveChangesAsync();
        }
    }
}
