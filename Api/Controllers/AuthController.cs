using Microsoft.AspNetCore.Mvc;
using Nonuso.Application.IServices;
using Nonuso.Messages.Api;

namespace Nonuso.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost]
        public async Task<IActionResult> Create(UserSignUpParamModel model)
        {
            await _authService.SignUpAsync(model);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserSignInParamModel model)
        {
            var result = await _authService.SignInAsync(model);
            return Ok(result);
        }
    }
}
