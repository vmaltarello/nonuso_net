using Nonuso.Api.Common;
using Nonuso.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Nonuso.Api.Exceptions
{
    public class ApiExceptionHandler(RequestDelegate next, ILogger<ApiExceptionHandler> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ApiExceptionHandler> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, "Exception occurred");

            context.Response.StatusCode = ex switch
            {
                EntityNotFoundException => StatusCodes.Status404NotFound,
                AuthUnauthorizedException => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };

            object response = ex switch
            {
                #if DEBUG
                _ => ApiResponse<object>.Error(ex.InnerException?.Message ?? ex.Message)
                #else
                _ => ApiResponse<object>.Error(ex.Message)
                #endif
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
