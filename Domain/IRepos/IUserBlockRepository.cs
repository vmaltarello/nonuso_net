using Nonuso.Domain.Entities;

namespace Nonuso.Domain.IRepos
{
    public interface IUserBlockRepository
    {
        Task<UserBlock?> GetByIdAsync(Guid id);
        Task CreateAsync(UserBlock entity);
        Task DeleteAsync(UserBlock entity);
        Task<IEnumerable<UserBlock>> CheckBlockAsync(Guid currentUserId, Guid otherUserId, Guid? conversationId = null);
    }
}
