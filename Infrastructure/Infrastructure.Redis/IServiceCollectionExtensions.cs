using Microsoft.Extensions.DependencyInjection;
using Nonuso.Application.IServices;
using Nonuso.Domain.IRepos;
using Nonuso.Infrastructure.Redis.Repos;
using Nonuso.Infrastructure.Redis.Services;
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

            services.AddScoped<IPresenceService, PresenceService>();
            services.AddScoped<IPresenceRepository, PresenceRepository>();
            return services;
        }
    }
}
