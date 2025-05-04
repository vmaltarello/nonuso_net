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
    //[Authorize]
    public class ProductController(IProductService productService, CurrentUser currentUser) : ApiControllerBase
    {
        private readonly IProductService _productService = productService;
        private readonly CurrentUser _currentUser = currentUser;

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(Guid id)
        {
            return Ok(await _productService.GetDetailsAsync(id, _currentUser.Id));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPopular()
        {
            return Ok(await _productService.GetAllPopularAsync(_currentUser.Id));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductParamModel model)
        {
            await _productService.CreateAsync(model);
            return Created();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _productService.DeleteAsync(id);
            return Created();
        }
    }
}
