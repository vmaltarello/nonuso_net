using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nonuso.Domain.IRepos;
using Nonuso.Infrastructure.Persistence.Repos;

namespace Nonuso.Infrastructure.Persistence
{
    public static partial class IServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructurePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<NonusoDbContext>(options =>
                options
                    .UseNpgsql(configuration.GetConnectionString("DefaultConnection"), npgsqlOptions => npgsqlOptions.UseNetTopologySuite())
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking),
                ServiceLifetime.Scoped);

            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IFavoriteRepository, FavoriteRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ILastSearchRepository, LastSearchRepository>();
            services.AddScoped<IProductRequestRepository, ProductRequestRepository>();
            services.AddScoped<IConversationRepository, ConversationRepository>();

            return services;
        }
    }
}
