using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            return Ok(await _authService.GetCurrentUserAsync(_currentUser.Id));
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserProfileById(Guid id)
        {
            return Ok(await _authService.GetUserProfileAsync(id));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserProfile()
        {
            return Ok(await _authService.GetUserProfileAsync(_currentUser.Id));
        }

        [HttpPost]
        public async Task<IActionResult> AuthWithGoogle(AuthGoogleParamModel model)
        {
            return Ok(await _authService.AuthWithGoogleAsync(model.IdToken));
        }

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

        [Authorize]
        [HttpPost]
        public new async Task<IActionResult> SignOut()
        {
            await _authService.SignOutAsync(_currentUser.Id);
            return NoContent();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete()
        {
            await _authService.DeleteAsync(_currentUser.Id);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> RefreshToken(RefreshTokenParamModel model)
        {
            return Ok(await _authService.RefreshTokenAsync(model));
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(AuthConfirmEmailParamModel model)
        {
            return Ok(await _authService.ConfirmEmailAsync(model));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(UserChangePasswordParamModel model)
        {
            await _authService.ChangePasswordAsync(model, _currentUser.Id);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RequestResetPassword(RequestResetPasswordParamModel model)
        {
            await _authService.RequestResetPasswordAsync(model);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordParamModel model)
        {
            return Ok(await _authService.ResetPasswordAsync(model));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangeUserName(UserChangeUserNameParamModel model)
        {
            model.UserId = _currentUser.Id;
            await _authService.ChangeUserNameAsync(model);
            return Ok();
        }
    }
}
