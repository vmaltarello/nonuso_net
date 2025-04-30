using Nonuso.Application.IServices;
using Nonuso.Common;
using Nonuso.Domain.Entities;
using Nonuso.Domain.IRepos;
using Nonuso.Messages.Api;

namespace Nonuso.Application.Services
{
    internal class LastSearchService(ILastSearchRepository lastSearchRepository) : ILastSearchService
    {
        readonly ILastSearchRepository _lastSearchRepository = lastSearchRepository;

        public async Task CreateAsync(Guid userId, string search)
        {
            var entity = new LastSearch() { UserId = userId, Search = search };
            await _lastSearchRepository.CreateAsync(entity);
        }

        public async Task<IEnumerable<LastSearchResultModel>> GetByUserId(Guid id)
        {
            var result = await _lastSearchRepository.GetByUserId(id);

            return result.To<LastSearchResultModel[]>();
        }
    }
}
