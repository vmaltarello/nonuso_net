using AutoMapper;
using Nonuso.Application.IServices;
using Nonuso.Common;
using Nonuso.Domain.Entities;
using Nonuso.Domain.Exceptions;
using Nonuso.Domain.IRepos;
using Nonuso.Messages.Api;

namespace Nonuso.Application.Services
{
    internal class ProductRequestService(
        IMapper mapper,
        IProductRepository productRepository,
        IProductRequestRepository productRequestRepository,
        IConversationRepository conversationRepository,
        IChatRepository chatRepository,
        INotificationService notificationService) : IProductRequestService
    {
        readonly IMapper _mapper = mapper;
        readonly IProductRepository _productRepository = productRepository;
        readonly IProductRequestRepository _productRequestRepository = productRequestRepository;
        readonly IConversationRepository _conversationRepository = conversationRepository;
        readonly INotificationService _notificationService = notificationService;
        readonly IChatRepository _chatRepository = chatRepository;


        public async Task CreateAsync(ProductRequestParamModel model)
        {
            var product = await _productRepository.GetByIdAsync(model.ProductId)
                ?? throw new EntityNotFoundException(nameof(Product), model.ProductId);

            var productRequest = model.To<ProductRequest>();

            productRequest.RequestedId = product.UserId;
            var conversation = new Conversation
            {
                ProductRequest = productRequest,
                Messages = 
                [ 
                    new Message()
                    {
                        Content = model.Message,
                        SenderId = productRequest.RequesterId
                     }
                ],
                ConversationsInfo = 
                [
                    new ConversationInfo() 
                    {
                        UserId = productRequest.RequesterId
                    },
                     new ConversationInfo()
                    {
                        UserId = productRequest.RequestedId
                    }
                ]
            };

            await _productRequestRepository.CreateAsync(productRequest);
            await _conversationRepository.CreateAsync(conversation);

            var otherUser = await _chatRepository.GetChatWithUserByConversationIdAsync(conversation.Id, productRequest.RequesterId);

            if (otherUser != null)
            {
                await _notificationService.SendPushNotificationAsync(new PusNotificationParamModel()
                {
                    ConversationId = conversation.Id,
                    UserId = otherUser.Id,
                    UserName = otherUser.UserName!,
                    Content = model.Message
                });
            }
        }
    }
}
