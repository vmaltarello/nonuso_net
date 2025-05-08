using FluentValidation;
using Nonuso.Domain.Entities;
using Nonuso.Domain.Validators.Base;
using Nonuso.Domain.Validators.Factory;

namespace Nonuso.Domain.Validators
{
    internal class ProductValidator : DomainValidator<Product>
    {
        internal override void ConfigureRules(IDomainValidatorFactory validatorFactory)
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
            RuleFor(x => x.CategoryId).NotEmpty();
            RuleFor(x => x.LocationName).NotEmpty();
            RuleFor(x => x.CreatedAt).NotEmpty();
        }
    }
}