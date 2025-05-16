using Microsoft.Extensions.DependencyInjection;

namespace Nonuso.Infrastructure.Realtime
{
    public static partial class IServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureRealtime(this IServiceCollection services)
        {
            services.AddSignalR();

            return services;
        }
    }
}
