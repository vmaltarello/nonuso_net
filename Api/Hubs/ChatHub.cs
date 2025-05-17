using Microsoft.AspNetCore.SignalR;
using Nonuso.Application.IServices;

namespace Nonuso.Api.Hubs
{
    public class ChatHub(IChatService chatService) : Hub
    {
        readonly IChatService _chatService = chatService;

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
            var userId = Context.UserIdentifier!;

            await _chatService.CreateAsync(conversationId, Guid.Parse(userId), content);

            await Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessage", new
            {
                senderId = userId,
                content,
                sentAt = DateTime.UtcNow
            });
        }
    }
}
