using AutoMapper;
using FluentValidation;
using Nonuso.Application.IServices;
using Nonuso.Domain.Entities;
using Nonuso.Domain.IRepos;
using Nonuso.Domain.Validators.Factory;
using Nonuso.Messages.Api;

namespace Nonuso.Application.Services
{
    internal class ReviewService(
        IMapper mapper,
        IDomainValidatorFactory validatorFactory,
        IReviewRepository reviewRepository) : IReviewService
    {
        readonly IMapper _mapper = mapper;
        readonly IDomainValidatorFactory _validatorFactory = validatorFactory;
        readonly IReviewRepository _reviewRepository= reviewRepository;

        public async Task CreateAsync(ReviewParamModel model)
        {
            var entity = _mapper.Map<Review>(model);

            _validatorFactory.GetValidator<Review>().ValidateAndThrow(entity);

            await _reviewRepository.CreateAsync(entity);
        }
    }
}
