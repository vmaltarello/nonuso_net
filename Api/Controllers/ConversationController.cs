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
    public class ConversationController(IConversationService conversationService, CurrentUser currentUser) : ApiControllerBase
    {
        private readonly IConversationService _conversationService = conversationService;
        private readonly CurrentUser _currentUser = currentUser;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {            
            return Ok(await _conversationService.GetAllAsync(_currentUser.Id));
        }
    }
}
