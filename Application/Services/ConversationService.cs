using AutoMapper;
using Nonuso.Application.IServices;
using Nonuso.Common;
using Nonuso.Domain.IRepos;
using Nonuso.Domain.Models;
using Nonuso.Messages.Api;

namespace Nonuso.Application.Services
{
    internal class ConversationService(
        IMapper mapper,
        IConversationRepository conversationRepository) : IConversationService
    {
        readonly IMapper _mapper = mapper;
        readonly IConversationRepository _conversationRepository = conversationRepository;

        public async Task<IEnumerable<ConversationResultModel>> GetAllAsync(Guid userId)
        {
            var result = await _conversationRepository.GetAllAsync(userId);

            return _mapper.Map<ConversationResultModel[]>(result);
        }

        public async Task<IEnumerable<ChatResultModel>> GetMessagesAsync(Guid id, Guid userId)
        {
           var result = await _conversationRepository.GetMessagesAsync(id, userId);

            return _mapper.Map<ChatResultModel[]>(result);
        }        
    }
}
