using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nonuso.Domain.IRepos;
using Nonuso.Infrastructure.Redis.Repos;
using StackExchange.Redis;

namespace Nonuso.Infrastructure.Redis
{
    public static partial class IServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureRedis(this IServiceCollection services, IConfiguration configuration)
        {
         

            services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!)
            );

            services.AddScoped<IPresenceRepository, PresenceRepository>();
            return services;
        }
    }
}
