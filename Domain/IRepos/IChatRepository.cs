using Nonuso.Domain.Models;

namespace Nonuso.Domain.IRepos
{
    public interface IChatRepository
    {
        Task<ChatModel?> GetByConversationIdAsync(Guid id, Guid userId);
    }
}
