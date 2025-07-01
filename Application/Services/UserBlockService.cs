using AutoMapper;
using FluentValidation;
using Nonuso.Application.IServices;
using Nonuso.Domain.Entities;
using Nonuso.Domain.Exceptions;
using Nonuso.Domain.IRepos;
using Nonuso.Domain.Validators.Factory;
using Nonuso.Messages.Api;

namespace Nonuso.Application.Services
{
    internal class UserBlockService(
        IMapper mapper,
        IDomainValidatorFactory validatorFactory,
        IUserBlockRepository userBlockRepository) : IUserBlockService
    {
        readonly IMapper _mapper = mapper;
        readonly IDomainValidatorFactory _validatorFactory = validatorFactory;
        readonly IUserBlockRepository _userBlockRepository = userBlockRepository;

        public async Task BlockAsync(UserBlockParamModel model)
        {
            var entity = _mapper.Map<UserBlock>(model);

            _validatorFactory.GetValidator<UserBlock>().ValidateAndThrow(entity);

            await _userBlockRepository.CreateAsync(entity);
        }

        public async Task<CheckUserBlockResultModel> CheckBlockAsync(CheckUserBlockParamModel model)
        {
            var result = await _userBlockRepository.CheckBlockAsync(model.CurrentUserId, model.OtherUserId, model.ConversationId);

            if (!result.Any()) return new CheckUserBlockResultModel();

            return new CheckUserBlockResultModel() 
            {
                Id = result.Where(x => x.BlockerId == model.CurrentUserId).First().Id,
                CurrentUserIsBlocked = result.Where(x => x.BlockerId == model.OtherUserId && x.BlockedId == model.CurrentUserId).Any(),
                OtherUserIsBlocked = result.Where(x => x.BlockerId == model.CurrentUserId && x.BlockedId == model.OtherUserId).Any(),
                ConversationId = result.Where(x => x.BlockerId == model.CurrentUserId).First().ConversationId,
            };
        }

        public async Task UnBlockAsync(Guid id)
        {
            var entity = await _userBlockRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException(nameof(UserBlock), id);

            await _userBlockRepository.DeleteAsync(entity);            
        }
    }
}
