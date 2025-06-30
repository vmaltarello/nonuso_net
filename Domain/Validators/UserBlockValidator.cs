using FluentValidation;
using Nonuso.Domain.Entities;
using Nonuso.Domain.Validators.Base;
using Nonuso.Domain.Validators.Factory;

namespace Nonuso.Domain.Validators
{
    internal class UserBlockValidator : DomainValidator<UserBlock>
    {
        internal override void ConfigureRules(IDomainValidatorFactory validatorFactory)
        {
            RuleFor(x => x.BlockerId).NotEmpty();
            RuleFor(x => x.BlockedId).NotEmpty();
            RuleFor(x => x.Reason).IsInEnum();
        }
    }
}
