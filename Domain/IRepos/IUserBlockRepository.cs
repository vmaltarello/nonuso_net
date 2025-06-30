using Nonuso.Domain.Entities;

namespace Nonuso.Domain.IRepos
{
    public interface IUserBlockRepository
    {
        Task<UserBlock?> GetByIdAsync(Guid id);
        Task CreateAsync(UserBlock entity);
        Task DeleteAsync(UserBlock entity);
    }
}
