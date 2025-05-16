using Microsoft.Extensions.DependencyInjection;
using Nonuso.Application.IServices;
using Nonuso.Infrastructure.Notification.Services;
namespace Nonuso.Infrastructure.Notification
{
    public static partial class IServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureNotification(this IServiceCollection services)
        {
            services.AddHttpClient<IOneSignalService, OneSignalService>();
            return services;
        }
    }
}
