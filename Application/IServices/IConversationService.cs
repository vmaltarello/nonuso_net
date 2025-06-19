using Nonuso.Messages.Api;

namespace Nonuso.Application.IServices
{
    public interface IConversationService
    {
        Task<ConversationResultModel> GetByIdAsync(Guid id, Guid userId);
        Task<IEnumerable<ConversationResultModel>> GetAllAsync(Guid userId);
        Task<ConversationResultModel?> GetActiveAsync(Guid productId, Guid userId);
        Task DeleteAsync(Guid id, Guid userId);
    }
}
