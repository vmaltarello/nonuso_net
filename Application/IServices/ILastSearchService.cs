namespace Nonuso.Application.IServices
{
    public interface ILastSearchService
    {
        Task CreateAsync(Guid userId, string search);
        Task<IEnumerable<string>> GetByUserId(Guid id);
    }
}
