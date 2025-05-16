using Microsoft.EntityFrameworkCore;
using Nonuso.Domain.Entities;
using Nonuso.Domain.IRepos;
using Nonuso.Domain.Models;

namespace Nonuso.Infrastructure.Persistence.Repos
{
    internal class ConversationRepository(NonusoDbContext context) : IConversationRepository
    {
        private readonly NonusoDbContext _context = context;

        public async Task<IEnumerable<Domain.Models.ConversationModel>> GetAllAsync(Guid userId)
        {
            return await _context.Conversation.Include(x => x.ProductRequest).ThenInclude(x => x.Product)
                .Where(x => x.ProductRequest != null 
                            && (x.ProductRequest.RequestedId == userId || x.ProductRequest.RequesterId == userId))
                
                .Select(x => new ConversationModel() 
                {
                    Product = x.ProductRequest!.Product!,
                    Id = x.Id,
                    CreatedAt = x.CreatedAt,
                    ProductRequestId = x.ProductRequestId
                })
                .ToListAsync();
        }
    }
}
