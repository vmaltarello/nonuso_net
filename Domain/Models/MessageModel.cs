namespace Nonuso.Domain.Models
{
    public class MessageModel
    {
        public Guid Id { get; set; }
        public required Guid SenderId { get; set; }
        public required string Content { get; set; }
        public bool IsAttachment { get; set; } = false;
        public DateTime CreatedAt { get; set; }
    }
}
