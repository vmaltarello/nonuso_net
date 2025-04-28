using Microsoft.Extensions.DependencyInjection;

namespace Nonuso.Application
{
    public static partial class IServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            //var mapperConfig = new MapperConfiguration(mc =>
            //{
            //    mc.AddProfile(new Mapping.Mapper());
            //});

            //IMapper mapper = mapperConfig.CreateMapper();
            //services.AddSingleton(mapper);

            //services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
