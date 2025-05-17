using Nonuso.Domain.Models;

namespace Nonuso.Domain.IRepos
{
    public interface IConversationRepository
    {
        Task<IEnumerable<ConversationModel>> GetAllAsync(Guid userId);
        Task<IEnumerable<ChatModel>> GetMessagesAsync(Guid id, Guid userId);
    }
}
