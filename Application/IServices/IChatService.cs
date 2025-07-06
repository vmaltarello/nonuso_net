using Nonuso.Domain.Entities;
using Nonuso.Messages.Api;

namespace Nonuso.Application.IServices
{
    public interface IChatService
    {
        Task<UserModel> GetChatWithUserByConversationIdAsync(Guid id, Guid userId);
        Task SetAllReaded(Guid conversationId, Guid userId);
        Task<MessageResultModel> CreateAsync(MessageParamModel model);
        Task<ChatResultModel> GetByConversationIdAsync(Guid id, Guid userId);
    }
}
