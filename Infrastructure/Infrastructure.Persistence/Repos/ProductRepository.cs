using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Nonuso.Common.Filters;
using Nonuso.Domain.Entities;
using Nonuso.Domain.IRepos;
using Nonuso.Domain.Models;

namespace Nonuso.Infrastructure.Persistence.Repos
{
    internal class ProductRepository(NonusoDbContext context) : IProductRepository
    {
        private readonly NonusoDbContext _context = context;

        public async Task<ProductDetailModel?> GetByIdAsync(Guid id, Guid userId)
        {
            var entity = await _context.Product.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return null;

            var favorites = await _context.Favorite.Where(x => x.ProductId == id).ToListAsync();
            var isMyFavorite = favorites.FirstOrDefault(x => x.UserId == userId) != null;
            var favoritesCount = favorites.Count;

            return new ProductDetailModel() 
            {
                Id = id,
                Title = entity.Title,
                Description = entity.Description,
                Location = entity.Location,
                LocationName = entity.LocationName,
                CategoryId = entity.CategoryId,
                CreatedAt = entity.CreatedAt,
                UserId = entity.UserId,
                IsMyFavorite = isMyFavorite,
                FavoriteCount = favoritesCount,
                ImagesUrl = entity.ImagesUrl,
                User = entity.User,
                IsMyProduct = entity.UserId == userId
            };
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Product.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Product>> GetAllPopularAsync(Guid? userId = null)
        {
            return await _context.Product
                .Where(x => x.IsEnabled && (userId == null || x.UserId != userId))
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

        public async Task<IEnumerable<Product>> GetAllActiveAsync(Guid userId)
        {
            return await _context.Product
                .Where(x => x.UserId == userId && x.IsEnabled)
                .Include(x => x.User)
                .OrderByDescending(x => x.UpdatedAt)
                .ThenByDescending(x => x.CreatedAt)
                .Take(5)
                .Select(x => x)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchAsync(ProductFilter filters)
        {
            var pageSize = 30;
            var skip = filters.Page * pageSize;     

            bool isLocationSearch = filters.Lat.HasValue && filters.Lon.HasValue && filters.Distance.HasValue;

            IQueryable<Product> baseQuery = _context.Product.Include(p => p.User).Where(p => p.IsEnabled);

            if (filters.CategoryId.HasValue)
                baseQuery = baseQuery.Where(p => p.CategoryId == filters.CategoryId.Value);

            if (filters.UserId.HasValue)
                baseQuery = baseQuery.Where(p => p.UserId != filters.UserId.Value);

            if (!string.IsNullOrEmpty(filters.Search))
            {
                baseQuery = baseQuery.Where(p => p.SearchVector.Matches(filters.Search));

            }

            var productsQuery = from p in baseQuery
                                join f in _context.Favorite.Where(f => filters.UserId.HasValue 
                                && f.UserId == filters.UserId.Value)
                                     on p.Id equals f.ProductId into userFavs
                                from uf in userFavs.DefaultIfEmpty()
                                select new
                                {
                                    Product = p,
                                    IsFavorite = uf != null,
                                    Distance = isLocationSearch
                                       ? p.Location.Distance(
                                           new Point(filters.Lon!.Value, filters.Lat!.Value) { SRID = 4326 }
                                         ) / 1000.0
                                       : 0.0
                                };

            if (isLocationSearch)
            {
                productsQuery = productsQuery.Where(p => p.Distance <= filters.Distance!.Value)
                    .OrderBy(x => x.Distance);

            }

            productsQuery = productsQuery
               .OrderBy(p => p.Product.UpdatedAt ?? p.Product.CreatedAt)
               .ThenByDescending(p => p.Product.CreatedAt);

            var paginatedProducts = await productsQuery
                .Skip(skip)
                .Take(pageSize)
                .Select(x => x.Product)
                .ToListAsync();

            return paginatedProducts;

        }

        public async Task CreateAsync(Product entity)
        {
            _context.Product.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;

            _context.Product.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product entity)
        {
            var favoritesToDelete = _context.Favorite.Where(x => x.ProductId == entity.Id);

            _context.Favorite.RemoveRange(favoritesToDelete);

            entity.UpdatedAt = DateTime.UtcNow;
            entity.IsEnabled = false;
            _context.Product.Update(entity);

            await _context.SaveChangesAsync();
        }
    }
}
