using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nonuso.Api.Common;
using Nonuso.Api.Controllers.Base;
using Nonuso.Application.IServices;
using Nonuso.Common.Filters;
using Nonuso.Messages.Api;

namespace Nonuso.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]    
    public class ProductController(IProductService productService, CurrentUser currentUser) : ApiControllerBase
    {
        private readonly IProductService _productService = productService;
        private readonly CurrentUser _currentUser = currentUser;

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _productService.GetByIdAsync(id, _currentUser.Id));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPopular()
        {
            return Ok(await _productService.GetAllPopularAsync(_currentUser.Id));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllActive()
        {
            return Ok(await _productService.GetAllActiveAsync(_currentUser.Id));
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] ProductFilter filters)
        {
            if (_currentUser.Id != Guid.Empty) filters.UserId = _currentUser.Id;

            return Ok(await _productService.SearchAsync(filters));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductParamModel model)
        {
            model.UserId = _currentUser.Id;

            await _productService.CreateAsync(model);
            return Ok();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Update([FromForm] ProductParamModel model)
        {
            model.UserId = _currentUser.Id;

            await _productService.UpdateAsync();
            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _productService.DeleteAsync(id);
            return NoContent();
        }
    }
}
