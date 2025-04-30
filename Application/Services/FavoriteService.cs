using Nonuso.Application.IServices;
using Nonuso.Common;
using Nonuso.Domain.Entities;
using Nonuso.Domain.Exceptions;
using Nonuso.Domain.IRepos;
using Nonuso.Messages.Api;

namespace Nonuso.Application.Services
{
    internal class FavoriteService(IFavoriteRepository favoriteRepository) : IFavoriteService
    {
        readonly IFavoriteRepository _favoriteRepository = favoriteRepository;

        public async Task<IEnumerable<FavoriteResultModel>> GetByUserIdAsync(Guid id)
        {
            var result = await _favoriteRepository.GetByUserIdAsync(id);

            return result.To<FavoriteResultModel[]>();
        }

        public async Task CreateAsync(Guid userId, Guid productId)
        {
            var entity = new Favorite() { UserId = userId, ProductId = productId };

            await _favoriteRepository.CreateAsync(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _favoriteRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException(nameof(Favorite), id);

            await _favoriteRepository.DeleteAsync(entity);
        }
    }
}
