using AutoMapper;
using Nonuso.Application.IServices;
using Nonuso.Domain.Entities;
using Nonuso.Domain.Exceptions;
using Nonuso.Domain.IRepos;
using Nonuso.Messages.Api;

namespace Nonuso.Application.Services
{
    internal class ChatService(
        IMapper mapper,
        IChatRepository chatRepository) : IChatService
    {
        readonly IMapper _mapper = mapper;
        readonly IChatRepository _chatRepository = chatRepository;

        public async Task CreateAsync(Guid conversationId, Guid userId, string content)
        {

        }

        public async Task<ChatResultModel> GetByConversationIdAsync(Guid id, Guid userId)
        {
            var result = await _chatRepository.GetByConversationIdAsync(id, userId)
                ?? throw new EntityNotFoundException(nameof(Conversation), id);

            return _mapper.Map<ChatResultModel>(result);
        }
    }
}
