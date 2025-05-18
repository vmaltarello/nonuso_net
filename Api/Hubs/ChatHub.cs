using Microsoft.AspNetCore.SignalR;
using Nonuso.Api.Common;
using Nonuso.Application.IServices;

namespace Nonuso.Api.Hubs
{
    public class ChatHub(IChatService chatService, CurrentUser currentUser) : Hub
    {
        readonly IChatService _chatService = chatService;
        readonly CurrentUser _currentUser = currentUser;

        public override async Task OnConnectedAsync()
        {
            var conversationId = Context.GetHttpContext()?.Request.Query["conversationId"];
            if (!string.IsNullOrWhiteSpace(conversationId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, conversationId!);
            }

            await base.OnConnectedAsync();
        }


        public async Task SendMessage(Guid conversationId, string content)
        {
            var result = await _chatService.CreateAsync(new Messages.Api.MessageParamModel() { ConversationId = conversationId, SenderId =
                            _currentUser.Id, Content = content});

            await Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessage", result);
        }
    }
}
