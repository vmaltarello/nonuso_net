using AutoMapper;
using FluentValidation;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.HPRtree;
using Nonuso.Application.IServices;
using Nonuso.Common;
using Nonuso.Common.Filters;
using Nonuso.Domain.Entities;
using Nonuso.Domain.Exceptions;
using Nonuso.Domain.IRepos;
using Nonuso.Domain.Models;
using Nonuso.Domain.Validators.Factory;
using Nonuso.Messages.Api;

namespace Nonuso.Application.Services
{
    internal class ProductService(
        IMapper mapper,
        IDomainValidatorFactory validatorFactory,
        IS3StorageService storageService,         
        IProductRepository productRepository,
        IProductRequestRepository productRequestRepository) : IProductService
    {
        readonly IMapper _mapper = mapper;
        readonly IDomainValidatorFactory _validatorFactory = validatorFactory;
        readonly IS3StorageService _storageService = storageService;
        readonly IProductRepository _productRepository = productRepository;
        readonly IProductRequestRepository _productRequestRepository = productRequestRepository;

        public async Task<ProductDetailResultModel> GetByIdAsync(Guid id, Guid userId)
        {
            var result = await _productRepository.GetByIdAsync(id, userId)
                ?? throw new EntityNotFoundException(nameof(ProductDetailModel), id);

            return _mapper.Map<ProductDetailResultModel>(result);
        }

        public async Task<IEnumerable<ProductResultModel>> GetAllPopularAsync(Guid? userId = null)
        {
            var result = await _productRepository.GetAllPopularAsync(userId);

            return _mapper.Map<ProductResultModel[]>(result);
        }

        public async Task<IEnumerable<ProductResultModel>> GetAllActiveAsync(Guid userId)
        {
            var result = await _productRepository.GetAllActiveAsync(userId);

            return _mapper.Map<ProductResultModel[]>(result);
        }

        public async Task<IEnumerable<ProductResultModel>> SearchAsync(ProductFilter filters)
        {
            var result = await _productRepository.SearchAsync(filters);

            return _mapper.Map<ProductResultModel[]>(result);
        }

        public async Task CreateAsync(ProductParamModel model)
        {
            var entity = model.To<Product>();
            entity.Title = char.ToUpper(entity.Title[0]) + entity.Title[1..];
            entity.Description = char.ToUpper(entity.Description[0]) + entity.Description[1..];

            var point = new Point(model.Longitude, model.Latitude)
            {
                SRID = 4326 // WGS84, standard for GPS
            };

            entity.Location = point;

            _validatorFactory.GetValidator<Product>().ValidateAndThrow(entity);

            await _productRepository.CreateAsync(entity);

            if(model.Images.Any())
            {
                var uploadedUrls = await _storageService.UploadProductImagesAsync(model.Images, entity.Id);

                entity.ImagesUrl = string.Join(",", uploadedUrls);

                await _productRepository.UpdateAsync(entity);
            }
        }

        public async Task UpdateAsync(EditProductParamModel model)
        {
            var entity = await _productRepository.GetByIdAsync(model.Id, model.UserId)
                ?? throw new EntityNotFoundException(nameof(ProductDetailModel), model.Id);

            entity.PopulateWith(model);

            entity.Title = char.ToUpper(entity.Title[0]) + entity.Title[1..];
            entity.Description = char.ToUpper(entity.Description[0]) + entity.Description[1..];

            _validatorFactory.GetValidator<Product>().ValidateAndThrow(entity);

            entity.ImagesUrl = string.Join(",", await _storageService.RemoveProductImagesAsync(model.ExistingImages, entity.Id));

            if (model.Images.Any())
            {
                var uploadedUrls = await _storageService.UploadProductImagesAsync(model.Images, entity.Id);

                var existingUrls = string.IsNullOrEmpty(entity.ImagesUrl)
                ? []
                : entity.ImagesUrl.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

                existingUrls.AddRange(uploadedUrls);
                entity.ImagesUrl = string.Join(",", existingUrls);
            }

            await _productRepository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(Guid id, Guid userId) 
        {
            var entity = await _productRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException(nameof(Product), id);

            var productRequests = await _productRequestRepository.GetByProductIdAsync(entity.Id, userId);

            if(productRequests.Any())
            {
                foreach (var item in productRequests) 
                {
                    item.Status = ProductRequestStatus.ProductUnavailable;
                    item.UpdatedAt = DateTime.UtcNow;
                }

                await _productRequestRepository.UpdateRangeAsync(productRequests);
            }

            await _productRepository.DeleteAsync(entity);
        }
    }
}
