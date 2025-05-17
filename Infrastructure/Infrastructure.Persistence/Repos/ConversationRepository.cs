using Microsoft.EntityFrameworkCore;
using Nonuso.Domain.IRepos;
using Nonuso.Domain.Models;

namespace Nonuso.Infrastructure.Persistence.Repos
{
    internal class ConversationRepository(NonusoDbContext context) : IConversationRepository
    {
        private readonly NonusoDbContext _context = context;

        public async Task<IEnumerable<Domain.Models.ConversationModel>> GetAllAsync(Guid userId)
        {

            return await _context.ProductRequest.Where(x => x.RequesterId == userId || x.RequestedId == userId)
                .Include(x => x.Product)
                .OrderByDescending(x => x.Conversation.CreatedAt)
                .Select(x => new ConversationModel() 
                {
                    Id = x.Conversation.Id,
                    ProductRequest = x,
                    CreatedAt = x.Conversation.CreatedAt,
                    LastMessage = x.Conversation.Messages.First().Content ?? string.Empty,
                    LastMessageDate = x.Conversation.Messages.First().CreatedAt,
                    ChatWithUser = x.RequestedId == userId ? x.RequestedUser! : x.RequesterUser!
                })
                .ToListAsync();           
        }
    }
}
