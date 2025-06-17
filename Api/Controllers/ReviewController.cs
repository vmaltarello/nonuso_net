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
    public class ReviewController(IReviewService reviewService, CurrentUser currentUser) : ApiControllerBase
    {
        private readonly IReviewService _reviewService = reviewService;
        private readonly CurrentUser _currentUser = currentUser;

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(ReviewParamModel model)
        {
            model.ReviewerUserId = _currentUser.Id;
            await _reviewService.CreateAsync(model);
            return Ok();
        }
    }
}
