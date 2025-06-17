using FluentValidation;
using Nonuso.Domain.Entities;
using Nonuso.Domain.Validators.Base;
using Nonuso.Domain.Validators.Factory;

namespace Nonuso.Domain.Validators
{
    internal class ReviewValidator : DomainValidator<Review>
    {
        internal override void ConfigureRules(IDomainValidatorFactory validatorFactory)
        {
            RuleFor(x => x.ReviewerUserId).NotEmpty();
            RuleFor(x => x.ReviewedUserId).NotEmpty();
            RuleFor(x => x.ProductRequestId).NotEmpty();

            RuleFor(x => x.Content)
                .NotEmpty()
                .MaximumLength(500)
                .When(x => !string.IsNullOrEmpty(x.Content));

            RuleFor(x => x.Stars).NotEmpty().InclusiveBetween(1, 5);
            RuleFor(x => x.CreatedAt).NotEmpty();
        }
    }
}