using AutoMapper;
using Nonuso.Application.IServices;
using Nonuso.Domain.Entities;
using Nonuso.Domain.Exceptions;
using Nonuso.Domain.IRepos;
using Nonuso.Messages.Api;

namespace Nonuso.Application.Services
{
    internal class FavoriteService(
        IMapper mapper,
        IFavoriteRepository favoriteRepository) : IFavoriteService
    {
        readonly IMapper _mapper = mapper;
        readonly IFavoriteRepository _favoriteRepository = favoriteRepository;       

        public async Task<IEnumerable<FavoriteResultModel>> GetByUserIdAsync(Guid id)
        {
            var result = await _favoriteRepository.GetByUserIdAsync(id);

            return _mapper.Map<FavoriteResultModel[]>(result);
        }

        public async Task CreateAsync(Guid userId, Guid productId)
        {
            var entity = new Favorite() { UserId = userId, ProductId = productId };

            await _favoriteRepository.CreateAsync(entity);
        }

        public async Task DeleteAsync(Guid userId, Guid productId)
        {
            var entity = await _favoriteRepository.GetByUserAndProductIdAsync(userId, productId)
                ?? throw new EntityNotFoundException(nameof(Favorite), productId);

            await _favoriteRepository.DeleteAsync(entity);
        }
    }
}
