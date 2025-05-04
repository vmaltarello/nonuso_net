using FluentValidation;
using System.Collections.Concurrent;
using System.Reflection;

namespace Nonuso.Domain.Validators.Factory
{
    public interface IDomainValidatorFactory
    {
        IValidator<T> GetValidator<T>();
    }

    internal class DomainValidatorFactory : IDomainValidatorFactory
    {
        private static readonly Lazy<DomainValidatorFactory> _instance = new(() => new DomainValidatorFactory());
        private readonly ConcurrentDictionary<Type, Lazy<IValidator>> _validators = new();
        private readonly ConcurrentDictionary<Type, Type> _validatorTypes = new();

        internal static DomainValidatorFactory Instance => _instance.Value;

        /// <summary>
        /// Gets a validator for the specified type T.
        /// </summary>
        /// <typeparam name="T">The type of entity to get a validator for</typeparam>
        /// <returns>An IValidator<T> instance for validating entities of type T</returns>
        /// <exception cref="InvalidOperationException">Thrown when no validator is registered for type T</exception>
        public IValidator<T> GetValidator<T>()
        {
            var entityType = typeof(T);

            var lazyValidator = _validators.GetOrAdd(
                entityType,
                type => new Lazy<IValidator>(() => CreateValidator(type), LazyThreadSafetyMode.ExecutionAndPublication)
            );

            return (IValidator<T>)lazyValidator.Value;
        }

        #region PRIVATE

        private DomainValidatorFactory()
        {
            Initialize();
        }

        private void Initialize()
        {
            var validatorTypes = Assembly.GetExecutingAssembly()
               .GetTypes()
               .Where(t => !t.IsAbstract && !t.IsInterface && t.GetInterfaces()
                   .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>)));

            foreach (var validatorType in validatorTypes)
            {
                var entityType = validatorType.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>))
                    .GetGenericArguments()[0];

                _validatorTypes.TryAdd(entityType, validatorType);
            }
        }

        private IValidator CreateValidator(Type entityType)
        {
            if (!_validatorTypes.TryGetValue(entityType, out var validatorType))
            {
                throw new InvalidOperationException($"No validator registered for type {entityType.Name}");
            }

            var validator = (IValidator)Activator.CreateInstance(validatorType)!;

            if (validator.GetType().GetMethod("ConfigureRules", BindingFlags.Instance | BindingFlags.NonPublic) is MethodInfo configureMethod)
            {
                configureMethod.Invoke(validator, [this]);
            }

            return validator;
        }

        #endregion
    }
}
