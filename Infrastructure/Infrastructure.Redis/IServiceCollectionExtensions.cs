using Microsoft.Extensions.DependencyInjection;
using Nonuso.Domain.IRepos;
using Nonuso.Infrastructure.Redis.Repos;
using Nonuso.Infrastructure.Secret;
using StackExchange.Redis;

namespace Nonuso.Infrastructure.Redis
{
    public static partial class IServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureRedis(this IServiceCollection services, ISecretManager secretManager)
        {
            var connectionString = secretManager.GetConnectionString("redis");

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(connectionString));

            services.AddScoped<IPresenceRepository, PresenceRepository>();
            return services;
        }
    }
}
