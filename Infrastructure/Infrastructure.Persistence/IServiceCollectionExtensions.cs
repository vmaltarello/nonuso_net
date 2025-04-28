using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nonuso.Infrastructure.Persistence
{
    public static partial class IServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructurePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<NonusoDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);


            return services;
        }
    }
}
