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
        INotificationService notificationService,
        IConversationRepository conversationRepository) : IChatService
    {
        readonly IMapper _mapper = mapper;
        readonly IChatRepository _chatRepository = chatRepository;
        readonly IConversationRepository _conversationRepository = conversationRepository;
        readonly INotificationService _notificationService = notificationService;

        public async Task SetAllReaded(Guid conversationId, Guid userId)
        {
            var conversation = await _conversationRepository.GetEntityByIdAsync(conversationId, userId)
                ?? throw new EntityNotFoundException(nameof(Conversation), conversationId);

            foreach (var item in conversation.ConversationsInfo)
            {
                item.UnreadCount = 0;
            }

            await _conversationRepository.UpdateAsync(conversation);
        }

        public async Task<MessageResultModel> CreateAsync(MessageParamModel model)
        {
            var entity = model.To<Message>();

            await _chatRepository.CreateAsync(entity);

            var result = await _chatRepository.GetMessageById(entity.Id);

            var otherUser = await _chatRepository.GetChatWithUserByConversationIdAsync(model.ConversationId, model.SenderId);

            if (otherUser != null)
            {
                await _notificationService.SendPushNotificationAsync(new PusNotificationParamModel()
                {
                    UserId = otherUser.Id,
                    Content = model.Content,
                    UserName = otherUser.UserName!,
                    ConversationId = model.ConversationId
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
