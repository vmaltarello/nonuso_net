using Nonuso.Domain.Entities;

namespace Nonuso.Domain.IRepos
{
    public interface IReviewRepository
    {
        Task CreateAsync(Review entity);
    }
}
