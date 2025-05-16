using Nonuso.Application.IServices;
using Nonuso.Common;
using Nonuso.Domain.Entities;
using Nonuso.Domain.Exceptions;
using Nonuso.Domain.IRepos;
using Nonuso.Messages.Api;

namespace Nonuso.Application.Services
{
    internal class ProductRequestService(IProductRepository productRepository,
        IProductRequestRepository productRequestRepository) : IProductRequestService
    {
        readonly IProductRepository _productRepository = productRepository;
        readonly IProductRequestRepository _productRequestRepository = productRequestRepository;

        public async Task<ProductRequestResultModel?> GetActiveAsync(Guid userId, Guid productId)
        {
            var result = await _productRequestRepository.GetActiveAsync(userId, productId);

            return result?.To<ProductRequestResultModel>();
        }

        public async Task CreateAsync(ProductRequestParamModel model)
        {
            var product = await _productRepository.GetByIdAsync(model.ProductId)
                ?? throw new EntityNotFoundException(nameof(Product), model.ProductId);

            var productRequest = model.To<ProductRequest>();

            productRequest.RequestedId = product.UserId;
            productRequest.Conversation = new Conversation
            {
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
        }
    }
}
