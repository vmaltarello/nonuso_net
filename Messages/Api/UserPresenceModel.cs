namespace Nonuso.Messages.Api
{
    public class UserPresenceModel
    {
        public required Guid UserId { get; set; }
        public required string CurrentPage { get; set; }
    }
}
