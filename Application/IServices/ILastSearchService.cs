using Nonuso.Messages.Api;

namespace Nonuso.Application.IServices
{
    public interface ILastSearchService
    {
        Task CreateAsync(Guid userId, string search);
        Task<IEnumerable<LastSearchResultModel>> GetByUserId(Guid id);
    }
}
