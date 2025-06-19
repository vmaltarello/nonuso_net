using Microsoft.EntityFrameworkCore;
using Nonuso.Domain.Entities;
using Nonuso.Domain.IRepos;
using Nonuso.Domain.Models;

namespace Nonuso.Infrastructure.Persistence.Repos
{
    internal class ChatRepository(NonusoDbContext context) : IChatRepository
    {
        private readonly NonusoDbContext _context = context;

        public async Task CreateAsync(Message entity)
        {
            _context.Message.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetChatWithUserByConversationIdAsync(Guid id, Guid userId)
        {
            return await _context.Conversation
                .Where(x => x.ProductRequest != null
                            && x.Id == id)
                .Select(x => 
                    x.ProductRequest!.RequestedId == userId ? x.ProductRequest!.RequesterUser! : x.ProductRequest!.RequestedUser!
                ).FirstOrDefaultAsync();
        }

        public async Task<MessageModel?> GetMessageById(Guid id)
        {
            return await _context.Message.Select(x => new MessageModel() 
            {
                Id = x.Id,
                Content = x.Content ?? string.Empty,
                IsAttachment = x.IsAttachment,
                CreatedAt = x.CreatedAt,
                SenderId = x.SenderId
            }).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ChatModel?> GetByConversationIdAsync(Guid id, Guid userId)
        {
            return await _context.Conversation
                .Where(x => x.ProductRequest != null 
                            && x.ProductRequest.Product != null 
                            && x.Id == id)
                .Include(x => x.ProductRequest).ThenInclude(x => x!.Product)
                .Select(x => new ChatModel()
                {
                    ProductRequest = x.ProductRequest!,
                    ChatWithUser = x.ProductRequest!.RequestedId == userId ? x.ProductRequest!.RequestedUser! : x.ProductRequest!.RequesterUser!,
                    Messages = x.Messages.OrderBy(x => x.CreatedAt).Select(x => new MessageModel()
                    {
                        Id = x.Id,
                        SenderId = x.SenderId,
                        Content = x.Content ?? string.Empty,
                        IsAttachment = x.IsAttachment,
                        CreatedAt = x.CreatedAt
                    })
                }).FirstOrDefaultAsync();
        }

    }
}
