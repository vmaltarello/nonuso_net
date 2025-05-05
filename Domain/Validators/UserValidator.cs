using FluentValidation;
using Nonuso.Domain.Entities;
using Nonuso.Domain.Validators.Base;
using Nonuso.Domain.Validators.Factory;

namespace Nonuso.Domain.Validators
{
    internal class UserValidator : DomainValidator<User>
    {
        internal override void ConfigureRules(IDomainValidatorFactory validatorFactory)
        {
            RuleFor(x => x.UserName).NotEmpty().MaximumLength(256);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        }
    }
}
