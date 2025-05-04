using Microsoft.EntityFrameworkCore;
using Nonuso.Common;
using Nonuso.Domain.Entities;
using Nonuso.Domain.IRepos;
using Nonuso.Domain.Models;

namespace Nonuso.Infrastructure.Persistence.Repos
{
    internal class ProductRepository(NonusoDbContext context) : IProductRepository
    {
        private readonly NonusoDbContext _context = context;

        public async Task<ProductDetailModel?> GetDetailsAsync(Guid id, Guid userId)
        {
            var entity = _context.Product.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return null;

            var favorites = await _context.Favorite.Where(x => x.ProductId == id).ToListAsync();
            var isMyFavorite = favorites.FirstOrDefault(x => x.UserId == userId) != null;
            var favoritesCount = favorites.Count;

            var result = entity.To<ProductDetailModel>();

            result.IsMyFavorite = isMyFavorite;
            result.FavoriteCount = favoritesCount;

            return result;
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Product.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Product>> GetAllPopularAsync(Guid? userId = null)
        {
            return await _context.Product
                .Where(x => userId == null || x.UserId != userId)
                .Include(x => x.User)
                .Select(x => new
                {
                    Product = x,
                    FavoriteCount = _context.Favorite
                        .Count(x => x.ProductId == x.Id && (userId == null || x.UserId != userId))
                })
                .OrderByDescending(x => x.FavoriteCount)
                .ThenByDescending(x => x.Product.UpdatedAt)
                .ThenByDescending(x => x.Product.CreatedAt)
                .Take(5)
                .Select(x => x.Product)
                .ToListAsync();           
        }

        public async Task CreateAsync(Product entity)
        {
            _context.Product.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product entity)
        {
            _context.Product.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product entity)
        {
            var favoritesToDelete = _context.Favorite.Where(x => x.ProductId == entity.Id);

            _context.Favorite.RemoveRange(favoritesToDelete);

            entity.IsEnabled = false;
            _context.Product.Update(entity);

            await _context.SaveChangesAsync();
        }
    }
}
