using Microsoft.AspNetCore.SignalR;
using Nonuso.Api.Common;
using Nonuso.Application.IServices;
using Nonuso.Messages.Api;

namespace Nonuso.Api.Hubs
{
    public class ChatHub(IChatService chatService, IPresenceService presenceService,
        INotificationService notificationService, CurrentUser currentUser) : Hub
    {
        readonly IChatService _chatService = chatService;
        readonly IPresenceService _presenceService = presenceService;
        readonly INotificationService _notificationService = notificationService;
        readonly CurrentUser _currentUser = currentUser;

        public override async Task OnConnectedAsync()
        {
            var conversationId = Context.GetHttpContext()?.Request.Query["conversationId"];
            if (!string.IsNullOrWhiteSpace(conversationId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, conversationId!);

                await _chatService.SetAllReaded(Guid.Parse(conversationId!), _currentUser.Id);
            }

            await base.OnConnectedAsync();
        }


        public async Task SendMessage(Guid conversationId, string content)
        {
            var result = await _chatService.CreateAsync(new Messages.Api.MessageParamModel()
            {
                ConversationId = conversationId,
                SenderId = _currentUser.Id,
                Content = content
            });

            await Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessage", result);

            var otherUser = await _chatService.GetChatWithUserByConversationIdAsync(conversationId, _currentUser.Id);

            var presence = await _presenceService.GetUserPresenceAsync(otherUser.Id);

            var notification = new PusNotificationParamModel()
            {
                UserId = otherUser.Id,
                Content = content,
                UserName = otherUser.UserName!,
                ConversationId = conversationId
            };

            // is offline --> send push notification
            if (!presence.HasValue || !presence.Value.currentPage.Contains(conversationId.ToString()))
            {
                await _notificationService.SendPushNotificationAsync(notification);
            }
            else
            {
                await Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessageInApp", notification);
            }
        }
    }
}
