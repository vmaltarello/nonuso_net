using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nonuso.Api.Common;
using Nonuso.Api.Controllers.Base;
using Nonuso.Application.IServices;
using Nonuso.Messages.Api;

namespace Nonuso.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserBlockController(IUserBlockService userBlockService, CurrentUser currentUser) : ApiControllerBase
    {
        private readonly IUserBlockService _userBlockService = userBlockService;
        private readonly CurrentUser _currentUser = currentUser;

        [HttpGet]
        public async Task<IActionResult> CheckBlock(CheckUserBlockParamModel model) 
        {
            model.CurrentUserId = _currentUser.Id;

            return Ok(await _userBlockService.CheckBlockAsync(model));

        }

        [HttpPost]
        public async Task<IActionResult> Block(UserBlockParamModel model)
        {
            model.BlockerId = _currentUser.Id;

            await _userBlockService.BlockAsync(model);
            return Ok();
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UnBlock(Guid id)
        {
            await _userBlockService.UnBlockAsync(id);
            return Ok();
        }
    }
}
