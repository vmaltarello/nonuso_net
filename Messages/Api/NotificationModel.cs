namespace Nonuso.Messages.Api
{
    public class PusNotificationParamModel
    {
        public required Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public Guid ProductRequestId { get; set; }
    }
}
