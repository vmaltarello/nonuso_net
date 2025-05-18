using Nonuso.Messages.Api;

namespace Nonuso.Application.IServices
{
    public interface IChatService
    {
        Task<MessageResultModel> CreateAsync(MessageParamModel model);
        Task<ChatResultModel> GetByConversationIdAsync(Guid id, Guid userId);
    }
}
