using AspNetCoreRateLimit;

namespace Nonuso.Api.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder SetupSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
                options.EnableDeepLinking();
                options.DisplayRequestDuration();
            });

            return app;
        }

        public static IApplicationBuilder AddUseIpRateLimiting(this IApplicationBuilder app) 
        {
            app.UseIpRateLimiting();

            return app;
        }
    }
}