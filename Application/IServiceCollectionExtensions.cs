using Microsoft.Extensions.DependencyInjection;
using Nonuso.Application.IServices;
using Nonuso.Application.Services;

namespace Nonuso.Application
{
    public static partial class IServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IFavoriteService, FavoriteService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ILastSearchService, LastSearchService>();

            return services;
        }
    }
}
