using Nonuso.Domain.Entities;
using Nonuso.Domain.Models;

namespace Nonuso.Domain.IRepos
{
    public interface IChatRepository
    {
        Task CreateAsync(Message entity);
        Task<User?> GetChatWithUserByConversationIdAsync(Guid id, Guid userId);
        Task<MessageModel?> GetMessageById(Guid id);
        Task<ChatModel?> GetByConversationIdAsync(Guid id, Guid userId);
    }
}
