using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nonuso.Domain.IRepos;
using Nonuso.Infrastructure.Persistence.Repos;
using Nonuso.Infrastructure.Secret;

namespace Nonuso.Infrastructure.Persistence
{
    public static partial class IServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructurePersistence(this IServiceCollection services, ISecretManager secretManager)
        {
            var connectionString = secretManager.GetConnectionString("dbConnection");
                
            services.AddDbContext<NonusoDbContext>(options =>
                options
                    .UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.UseNetTopologySuite())
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking),
                ServiceLifetime.Scoped);

            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IFavoriteRepository, FavoriteRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ILastSearchRepository, LastSearchRepository>();
            services.AddScoped<IProductRequestRepository, ProductRequestRepository>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IUserBlockedRepository, UserBlockedRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();

            return services;
        }
    }
}
