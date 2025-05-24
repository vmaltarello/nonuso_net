using Nonuso.Application.IServices;
using Nonuso.Domain.Entities;
using Nonuso.Domain.IRepos;

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

        public async Task<IEnumerable<string>> GetByUserId(Guid id)
        {
            return await _lastSearchRepository.GetByUserId(id);
        }
    }
}
