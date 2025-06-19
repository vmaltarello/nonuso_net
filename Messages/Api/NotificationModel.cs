namespace Nonuso.Messages.Api
{
    public class PusNotificationParamModel
    {
        public required Guid UserId { get; set; }
        public required string UserName { get; set; }
        public required string Content { get; set; }
        public required Guid ConversationId { get; set; }
    }
}
