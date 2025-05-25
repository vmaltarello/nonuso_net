using Microsoft.AspNetCore.SignalR;
using Nonuso.Api.Common;
using Nonuso.Application.IServices;

namespace Nonuso.Api.Hubs
{
    public class PresenceHub(IPresenceService presenceService, CurrentUser currentUser) : Hub
    {
        readonly IPresenceService _presenceService = presenceService;
        readonly CurrentUser _currentUser = currentUser;

        public override async Task OnConnectedAsync()
        {
            var currentPage = Context.GetHttpContext()?.Request.Query["page"].ToString() ?? "unknown";
            
            await _presenceService.SetUserOnlineAsync(_currentUser.Id, currentPage);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _presenceService.SetUserOfflineAsync(_currentUser.Id);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task UpdatePageAsync(string page)
        {
            await _presenceService.UpdatePageAsync(_currentUser.Id, page);
        }
    }
}
