using Microsoft.EntityFrameworkCore;
using Nonuso.Domain.Entities;
using Nonuso.Domain.IRepos;
using Nonuso.Domain.Models;

namespace Nonuso.Infrastructure.Persistence.Repos
{
    internal class AuthRepository(NonusoDbContext context) : IAuthRepository
    {
        private readonly NonusoDbContext _context = context;

        public async Task<UserProfileModel> GetUserProfileAsync(Guid id)
        {
            var reviews = await _context.Review.Where(x => x.ReviewedUserId == id).ToListAsync();

            var joinedAt = await _context.Users.Where(x => x.Id == id).Select(x => x.CreatedAt).FirstOrDefaultAsync();

            var totalProducts = await _context.Product.Where(x => x.UserId == id).CountAsync();

            return new UserProfileModel() 
            { 
                Reviews = reviews, 
                JoinedMonth = joinedAt.Month.ToString(), 
                JoinedYear = joinedAt.Year.ToString(),
                ProductCount = totalProducts 
            };
        }

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
            entity.Email += $"_deleted_{entity.Id}";

            _context.Users.Update(entity);

            var tokenToRevoke = _context.RefreshToken.Where(x => x.UserId == entity.Id).ExecuteUpdate(x =>
            x.SetProperty(x => x.Revoked, true));

            await _context.SaveChangesAsync();
        }
    }
}
