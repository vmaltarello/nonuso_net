using Nonuso.Messages.Api;

namespace Nonuso.Application.IServices
{
    public interface IConversationService
    {
        Task<IEnumerable<ConversationResultModel>> GetAllAsync(Guid userId);
        Task<IEnumerable<ChatResultModel>> GetMessagesAsync(Guid id, Guid userId);
    }
}
