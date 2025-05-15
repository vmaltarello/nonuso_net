using AutoMapper;
using FluentValidation;
using NetTopologySuite.Geometries;
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
        IProductRepository productRepository) : IProductService
    {
        readonly IMapper _mapper = mapper;
        readonly IDomainValidatorFactory _validatorFactory = validatorFactory;
        readonly IS3StorageService _storageService = storageService;
        readonly IProductRepository _productRepository = productRepository;

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

        public async Task DeleteAsync(Guid id) 
        {
            var entity = await _productRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException(nameof(Product), id);

            await _productRepository.DeleteAsync(entity);
        }
    }
}
