using Microsoft.AspNetCore.Mvc;
using Nonuso.Api.Common;

namespace Nonuso.Api.Controllers.Base
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected IActionResult Ok<T>(T? result = default)
        {
            return base.Ok(ApiResponse<T>.Ok(result));
        }

        protected IActionResult BadRequest<T>(T? result = default, string message = "")
        {
            return base.BadRequest(ApiResponse<T>.Error(result, message));
        }

        protected IActionResult NotFound<T>(T? result = default, string message = "")
        {
            return base.NotFound(ApiResponse<T>.Error(result, message));
        }
    }
}
