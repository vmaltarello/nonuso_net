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
    public class ProductController(IProductService productService, CurrentUser currentUser) : ApiControllerBase
    {
        private readonly IProductService _productService = productService;
        private readonly CurrentUser _currentUser = currentUser;

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductParamModel model)
        {
            await _productService.CreateAsync(model);
            return Created();
        }
    }
}
