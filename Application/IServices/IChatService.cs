using Nonuso.Messages.Api;

namespace Nonuso.Application.IServices
{
    public interface IChatService
    {
        Task CreateAsync(Guid conversationId, Guid userId, string content);
        Task<ChatResultModel> GetByConversationIdAsync(Guid id, Guid userId);
    }
}
