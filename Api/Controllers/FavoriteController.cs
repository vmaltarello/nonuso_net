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
    [Authorize]
    public class FavoriteController(IFavoriteService favoriteService, CurrentUser currentUser) : ApiControllerBase
    {
        private readonly IFavoriteService _favoriteService = favoriteService;
        private readonly CurrentUser _currentUser = currentUser;

        [HttpGet]
        public async Task<IActionResult> GetByUserId() 
        {
            return Ok(await _favoriteService.GetByUserIdAsync(_currentUser.Id));
        }

        [HttpPost("{productId}")]
        public async Task<IActionResult> Create(Guid productId)
        {
            await _favoriteService.CreateAsync(_currentUser.Id, productId);
            return Created();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _favoriteService.DeleteAsync(id);
            return NoContent();
        }
    }
}
