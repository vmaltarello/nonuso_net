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
    public class ProductRequestController(IProductRequestService productRequestService, CurrentUser currentUser) : ApiControllerBase
    {
        private readonly IProductRequestService _productRequestService = productRequestService;
        private readonly CurrentUser _currentUser = currentUser;

        [HttpPost]
        public async Task<IActionResult> Create(ProductRequestParamModel model)
        {
            model.RequesterId = _currentUser.Id;

            await _productRequestService.CreateAsync(model);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetActive([FromQuery] Guid productId)
        {
            return Ok(await _productRequestService.GetActiveAsync(_currentUser.Id, productId));
        }
    }
}
