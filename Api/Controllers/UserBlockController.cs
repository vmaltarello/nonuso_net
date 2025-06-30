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

        [HttpPost]
        public async Task<IActionResult> Block(UserBlockParamModel model)
        {
            model.BlockerId = _currentUser.Id;

            await _userBlockService.Block(model);
            return Ok();
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UnBlock(Guid id)
        {
            await _userBlockService.UnBlock(id);
            return Ok();
        }
    }
}
