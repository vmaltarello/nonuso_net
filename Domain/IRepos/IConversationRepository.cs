using Nonuso.Domain.Entities;
using Nonuso.Domain.Models;

namespace Nonuso.Domain.IRepos
{
    public interface IConversationRepository
    {
        Task CreateAsync(Conversation entity);
        Task<IEnumerable<ConversationModel>> GetAllAsync(Guid userId);
        Task<ConversationModel?> GetActiveAsync(Guid productId, Guid userId);
        Task<Conversation?> GetByIdAsync(Guid id, Guid? userId);
        Task UpdateAsync(Conversation entity);
    }
}
