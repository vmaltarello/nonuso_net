using FluentValidation;
using Nonuso.Application.IServices;
using Nonuso.Common;
using Nonuso.Domain.Entities;
using Nonuso.Domain.Exceptions;
using Nonuso.Domain.IRepos;
using Nonuso.Domain.Models;
using Nonuso.Domain.Validators.Factory;
using Nonuso.Messages.Api;

namespace Nonuso.Application.Services
{
    internal class ProductService(
        IDomainValidatorFactory validatorFactory,
        IS3StorageService storageService,         
        IProductRepository productRepository) : IProductService
    {
        readonly IDomainValidatorFactory _validatorFactory = validatorFactory;
        readonly IS3StorageService _storageService = storageService;
        readonly IProductRepository _productRepository = productRepository;

        public async Task<ProductDetailResultModel> GetDetailsAsync(Guid id, Guid userId)
        {
            var result = await _productRepository.GetDetailsAsync(id, userId)
                ?? throw new EntityNotFoundException(nameof(ProductDetailModel), id);

            return result.To<ProductDetailResultModel>();
        }

        public async Task<IEnumerable<ProductResultModel>> GetAllPopularAsync(Guid? userId = null)
        {
            var result = await _productRepository.GetAllPopularAsync(userId);

            return result.To<ProductResultModel[]>();
        }

        public async Task CreateAsync(ProductParamModel model)
        {
            var entity = model.To<Product>();

            _validatorFactory.GetValidator<Product>().ValidateAndThrow(entity);

            await _productRepository.CreateAsync(entity);

            var uploadedUrls = await _storageService.UploadProductImagesAsync(model.Images, entity.Id);

            entity.ImagesUrl = string.Join(",", uploadedUrls);
            
            await _productRepository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(Guid id) 
        {
            var entity = await _productRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException(nameof(Product), id);

            await _productRepository.DeleteAsync(entity);
        }
    }
}
