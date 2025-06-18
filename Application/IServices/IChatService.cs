using Nonuso.Messages.Api;

namespace Nonuso.Application.IServices
{
    public interface IChatService
    {
        Task SetAllReaded(Guid conversationId, Guid userId);
        Task<MessageResultModel> CreateAsync(MessageParamModel model);
        Task<ChatResultModel> GetByConversationIdAsync(Guid id, Guid userId);
    }
}
