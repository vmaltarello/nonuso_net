using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nonuso.Api.Common;
using Nonuso.Api.Controllers.Base;
using Nonuso.Application.IServices;

namespace Nonuso.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ChatController(IChatService chatService, CurrentUser currentUser) : ApiControllerBase
    {
        private readonly IChatService _chatService = chatService;
        private readonly CurrentUser _currentUser = currentUser;

        [HttpGet("{id}")]
        public async Task<IActionResult> GetChat(Guid id)
        {
            return Ok(await _chatService.GetByConversationIdAsync(id, _currentUser.Id));
        }
    }
}
