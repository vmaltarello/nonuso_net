using Microsoft.EntityFrameworkCore;
using Nonuso.Domain.Entities;
using Nonuso.Domain.IRepos;
using Nonuso.Domain.Models;

namespace Nonuso.Infrastructure.Persistence.Repos
{
    internal class ConversationRepository(NonusoDbContext context) : IConversationRepository
    {
        private readonly NonusoDbContext _context = context;

        public async Task CreateAsync(Conversation entity)
        {
            _context.Conversation.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<ConversationModel?> GetActiveAsync(Guid productId, Guid userId)
        {
            return await _context.Conversation
               .Where(x => x.ProductRequest != null 
                           && x.ProductRequest.Product != null
                           && x.ProductRequest.Product.Id == productId
                           && (x.ProductRequest.RequestedId == userId || x.ProductRequest.RequesterId == userId))
               .Include(x => x.ProductRequest).ThenInclude(x => x!.Product).ThenInclude(x => x!.User)
               .OrderByDescending(x => x.CreatedAt)
               .Select(x => new ConversationModel()
               {
                   Id = x.Id,
                   ProductRequest = x.ProductRequest!,
                   CreatedAt = x.CreatedAt,
                   LastMessage = x.Messages.OrderByDescending(x => x.CreatedAt).First().Content ?? string.Empty,
                   LastMessageDate = x.Messages.OrderByDescending(x => x.CreatedAt).First().CreatedAt,
                   Messages = x.Messages.OrderBy(x => x.CreatedAt).Select(x => new MessageModel()
                   {
                       Id = x.Id,
                       IsMine = x.SenderId == userId,
                       Content = x.Content ?? string.Empty,
                       IsAttachment = x.IsAttachment,
                       CreatedAt = x.CreatedAt
                   }),
                   ChatWithUser = x.ProductRequest!.RequestedId == userId ? x.ProductRequest.RequesterUser! : x.ProductRequest.RequestedUser!
               })
               .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Domain.Models.ConversationModel>> GetAllAsync(Guid userId)
        {
            return await _context.Conversation
                .Where(x => x.ProductRequest != null &&
                            (x.ProductRequest.RequestedId == userId || x.ProductRequest.RequesterId == userId))
                .Include(x => x.ProductRequest).ThenInclude(x => x!.Product).ThenInclude(x => x!.User)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new ConversationModel()
                {
                    Id = x.Id,
                    ProductRequest = x.ProductRequest!,
                    CreatedAt = x.CreatedAt,
                    LastMessage = x.Messages.OrderByDescending(x => x.CreatedAt).First().Content ?? string.Empty,
                    LastMessageDate = x.Messages.OrderByDescending(x => x.CreatedAt).First().CreatedAt,
                    Messages = x.Messages.OrderBy(x => x.CreatedAt).Select(x => new MessageModel()
                    {
                        Id = x.Id,
                        IsMine = x.SenderId == userId,
                        Content = x.Content ?? string.Empty,
                        IsAttachment = x.IsAttachment,
                        CreatedAt = x.CreatedAt
                    }),
                    ChatWithUser = x.ProductRequest!.RequestedId == userId ? x.ProductRequest.RequesterUser! : x.ProductRequest.RequestedUser!
                })
                .ToListAsync();       
        }
    }
}
