using Microsoft.Extensions.DependencyInjection;

namespace Nonuso.Infrastructure.Secret
{
    public static partial class IServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureSecret(this IServiceCollection services)
        {
            services.AddScoped<ISecretManager, SecretManager>();
            return services;
        }
    }
}
