using Microsoft.Extensions.DependencyInjection;
using Nonuso.Domain.Validators.Factory;

namespace Nonuso.Domain
{
    public static partial class IServiceCollectionExtensions
    {
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddSingleton<IDomainValidatorFactory>(_ => DomainValidatorFactory.Instance);
            return services;
        }
    }
}
