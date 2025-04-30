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
    public class LastSearchController(ILastSearchService lastSearchService, CurrentUser currentUser) : ApiControllerBase
    {
        private readonly ILastSearchService _lastSearchService = lastSearchService;
        private readonly CurrentUser _currentUser = currentUser;

        [HttpPost]
        public async Task<IActionResult> Create(string search)
        {
            await _lastSearchService.CreateAsync(_currentUser.Id, search);
            return Created();
        }

        [HttpGet]
        public async Task<IActionResult> GetByUserId()
        {
            return Ok(await _lastSearchService.GetByUserId(_currentUser.Id));
        }
    }
}
