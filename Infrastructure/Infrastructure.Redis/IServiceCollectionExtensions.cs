using Microsoft.Extensions.DependencyInjection;
using Nonuso.Domain.IRepos;
using Nonuso.Infrastructure.Redis.Repos;
using StackExchange.Redis;

namespace Nonuso.Infrastructure.Redis
{
    public static partial class IServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureRedis(this IServiceCollection services)
        {
         

            services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect("localhost:6379")
            );

            services.AddScoped<IPresenceRepository, PresenceRepository>();
            return services;
        }
    }
}
