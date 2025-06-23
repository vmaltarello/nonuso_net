using Nonuso.Domain.Entities;

namespace Nonuso.Domain.IRepos
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetAllAsync(Guid userId);
        Task CreateAsync(Review entity);
    }
}
