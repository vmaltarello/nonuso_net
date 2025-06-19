using AutoMapper;
using Nonuso.Application.IServices;
using Nonuso.Domain.Entities;
using Nonuso.Domain.Exceptions;
using Nonuso.Domain.IRepos;
using Nonuso.Messages.Api;

namespace Nonuso.Application.Services
{
    internal class ConversationService(
        IMapper mapper,
        IConversationRepository conversationRepository) : IConversationService
    {
        readonly IMapper _mapper = mapper;
        readonly IConversationRepository _conversationRepository = conversationRepository;

        public async Task<ConversationResultModel> GetByIdAsync(Guid id, Guid userId)
        {
            var result = await _conversationRepository.GetByIdAsync(id, userId)
                ?? throw new EntityNotFoundException(nameof(Conversation), id);

            return _mapper.Map<ConversationResultModel>(result);
        }

        public async Task<IEnumerable<ConversationResultModel>> GetAllAsync(Guid userId)
        {
            var result = await _conversationRepository.GetAllAsync(userId);

            return _mapper.Map<ConversationResultModel[]>(result);
        }

        public async Task<ConversationResultModel?> GetActiveAsync(Guid productId, Guid userId)
        {
            var result = await _conversationRepository.GetActiveAsync(productId, userId);

            return _mapper.Map<ConversationResultModel?>(result);
        }

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            var entity = await _conversationRepository.GetEntityByIdAsync(id, userId)
                ?? throw new EntityNotFoundException(nameof(Conversation), id);

            foreach (var info in entity.ConversationsInfo)
            {
                info.Visible = false;
            }

            await _conversationRepository.UpdateAsync(entity);
        }
    }
}
