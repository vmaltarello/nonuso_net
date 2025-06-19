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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _conversationService.GetByIdAsync(id, _currentUser.Id));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {            
            return Ok(await _conversationService.GetAllAsync(_currentUser.Id));
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetActive(Guid productId)
        {
            return Ok(await _conversationService.GetActiveAsync(productId, _currentUser.Id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _conversationService.DeleteAsync(id, _currentUser.Id);
            return Ok();
        }
    }
}
