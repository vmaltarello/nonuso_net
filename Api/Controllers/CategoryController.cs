using Microsoft.AspNetCore.Mvc;
using Nonuso.Api.Common;
using Nonuso.Api.Controllers.Base;
using Nonuso.Application.IServices;

namespace Nonuso.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoryController(ICategoryService categoryService, CurrentUser currentUser) : ApiControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;
        private readonly CurrentUser _currentUser = currentUser;

        [HttpGet]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)] // Cache 24 hours
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _categoryService.GetAllAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPopular()
        {
            await _categoryService.GetAllPopularAsync(_currentUser.Id);
            return Ok();
        }
    }
}
