namespace Nonuso.Domain.Models
{
    public class MessageModel
    {
        public bool IsMine { get; set; } = false;
        public required string Content { get; set; }
        public bool IsAttachment { get; set; } = false;
        public DateTime CreatedAt { get; set; }
    }
}
