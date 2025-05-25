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
    public class NotificationController(INotificationService notificationService, CurrentUser currentUser) : ApiControllerBase
    {
        private readonly INotificationService _notificationService = notificationService;
        private readonly CurrentUser _currentUser = currentUser;

        [HttpPost]
        public async Task<IActionResult> SendPushNotification(PusNotificationParamModel model)
        {
            await _notificationService.SendPushNotificationAsync(model);
            return NoContent();
        }
    }
}
