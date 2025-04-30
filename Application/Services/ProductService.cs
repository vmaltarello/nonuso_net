using Nonuso.Application.IServices;
using Nonuso.Common;
using Nonuso.Domain.Entities;
using Nonuso.Domain.IRepos;
using Nonuso.Messages.Api;

namespace Nonuso.Application.Services
{
    internal class ProductService(IProductRepository productRepository) : IProductService
    {
        readonly IProductRepository _productRepository = productRepository;

        public async Task CreateAsync(ProductParamModel model)
        {
            var entity = model.To<Product>();

            await _productRepository.CreateAsync(entity);
        }
    }
}
