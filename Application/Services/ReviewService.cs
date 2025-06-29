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

        public async Task<IEnumerable<ReviewResultModel>> GetAllAsync(Guid userId)
        {
            var result = await _reviewRepository.GetAllAsync(userId);

            return _mapper.Map<ReviewResultModel[]>(result);
        }

        public async Task CreateAsync(ReviewParamModel model)
        {
            var entity = _mapper.Map<Review>(model);

            if (string.IsNullOrWhiteSpace(entity.Content)) entity.Content = null;

            _validatorFactory.GetValidator<Review>().ValidateAndThrow(entity);

            await _reviewRepository.CreateAsync(entity);
        }
    }
}
