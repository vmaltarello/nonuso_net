using AutoMapper;
using Nonuso.Application.IServices;
using Nonuso.Common;
using Nonuso.Domain.Entities;
using Nonuso.Domain.Exceptions;
using Nonuso.Domain.IRepos;
using Nonuso.Messages.Api;

namespace Nonuso.Application.Services
{
    internal class ChatService(
        IMapper mapper,
        IChatRepository chatRepository,
        INotificationService notificationService) : IChatService
    {
        readonly IMapper _mapper = mapper;
        readonly IChatRepository _chatRepository = chatRepository;
        readonly INotificationService _notificationService = notificationService;

        public async Task<MessageResultModel> CreateAsync(MessageParamModel model)
        {
            var entity = model.To<Message>();

            await _chatRepository.CreateAsync(entity);

            var result = await _chatRepository.GetMessageById(entity.Id, model.SenderId);

            var otherUser = await _chatRepository.GetChatWithUserByConversationIdAsync(model.ConversationId, model.SenderId);

            if (otherUser != null) 
            {
                await _notificationService.SendPushNotificationAsync(new PusNotificationParamModel()
                {
                    UserId = otherUser.Id,
                    Content = model.Content ?? string.Empty,
                    ProductRequestId = entity.Conversation!.ProductRequestId,
                    UserName = otherUser.UserName ?? string.Empty,
                });
            }

            return _mapper.Map<MessageResultModel>(result);
        }

        public async Task<ChatResultModel> GetByConversationIdAsync(Guid id, Guid userId)
        {
            var result = await _chatRepository.GetByConversationIdAsync(id, userId)
                ?? throw new EntityNotFoundException(nameof(Conversation), id);

            return _mapper.Map<ChatResultModel>(result);
        }
    }
}
