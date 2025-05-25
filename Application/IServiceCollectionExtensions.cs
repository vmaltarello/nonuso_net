using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Nonuso.Application.IServices;
using Nonuso.Application.Services;

namespace Nonuso.Application
{
    public static partial class IServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new Mapper());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IFavoriteService, FavoriteService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ILastSearchService, LastSearchService>();
            services.AddScoped<IProductRequestService, ProductRequestService>();
            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IUserBlockedService, UserBlockedService>();
            services.AddScoped<IPresenceService, PresenceService>();

            return services;
        }
    }
}
