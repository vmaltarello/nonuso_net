using Microsoft.AspNetCore.Mvc;
using Nonuso.Api.Common;
using Nonuso.Api.Controllers.Base;
using Nonuso.Application.IServices;
using Nonuso.Messages.Api;

namespace Nonuso.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController(IAuthService authService, CurrentUser currentUser) : ApiControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly CurrentUser _currentUser = currentUser;

        [HttpPost]
        public async Task<IActionResult> SignUp(UserSignUpParamModel model)
        {
            await _authService.SignUpAsync(model);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(UserSignInParamModel model)
        {
            return Ok(await _authService.SignInAsync(model));
        }

        [HttpPost]
        public new async Task<IActionResult> SignOut()
        {
            await _authService.SignOutAsync(_currentUser.Id);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> RefreshToken(RefreshTokenParamModel model)
        {
            return Ok(await _authService.RefreshTokenAsync(_currentUser.Id, model.RefreshToken));
        }

        [HttpGet]
        public async Task<IActionResult> UserNameIsUnique(string userName)
        {
            return Ok(await _authService.UserNameIsUniqueAsync(userName));
        }
    }
}
