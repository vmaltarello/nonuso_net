using FluentValidation;
using FluentValidation.Results;
using Nonuso.Domain.Validators.Factory;

namespace Nonuso.Domain.Validators.Base
{
    /// <summary>
    /// Base abstract class for domain validators that provides common validation functionality.
    /// Extends FluentValidation's AbstractValidator.
    /// </summary>
    /// <typeparam name="T">The type of entity to validate</typeparam>
    /// <remarks>
    /// This class serves as the base for all domain validators in the system.
    /// It provides common validation behavior and requires derived classes to configure their specific validation rules.
    /// </remarks>
    internal abstract class DomainValidator<T> : AbstractValidator<T>
    {
        internal abstract void ConfigureRules(IDomainValidatorFactory validatorFactory);

        protected override void RaiseValidationException(ValidationContext<T> context, ValidationResult result)
        {
            throw new ValidationException(result.Errors);
        }
    }
}
