using Nonuso.Domain.Entities;

namespace Nonuso.Domain.IRepos
{
    public interface ILastSearchRepository
    {
        Task CreateAsync(LastSearch entity);
        Task<IEnumerable<LastSearch>> GetByUserId(Guid id);
    }
}
