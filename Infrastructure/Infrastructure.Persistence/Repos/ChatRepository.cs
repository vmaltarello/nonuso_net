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

        public async Task<MessageModel?> GetMessageById(Guid id, Guid userId)
        {
            return await _context.Message.Select(x => new MessageModel() 
            {
                Id = x.Id,
                Content = x.Content ?? string.Empty,
                IsAttachment = x.IsAttachment,
                CreatedAt = x.CreatedAt,
                IsMine = x.SenderId == userId
            }).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ChatModel?> GetByConversationIdAsync(Guid id, Guid userId)
        {
            return await _context.ProductRequest.Where(x => x.Conversation.Id == id).Include(x => x.Product)
                .Select(x => new ChatModel()
                {
                    ProductRequest = x,
                    ChatWithUser = x.RequestedId == userId ? x.RequestedUser! : x.RequesterUser!,
                    Messages = x.Conversation.Messages.OrderBy(x => x.CreatedAt).Select(x => new MessageModel()
                    {
                        Id = x.Id,
                        IsMine = x.SenderId == userId,
                        Content = x.Content ?? string.Empty,
                        IsAttachment = x.IsAttachment,
                        CreatedAt = x.CreatedAt
                    })
                }).FirstOrDefaultAsync();
        }

    }
}
