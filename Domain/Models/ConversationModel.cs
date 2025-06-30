using Nonuso.Domain.Entities;

namespace Nonuso.Domain.Models
{
    public class ConversationModel
    {
        public Guid Id { get; set; }
        public required string LastMessage { get; set; }
        public DateTime LastMessageDate { get; set; }
        public IEnumerable<MessageModel> Messages { get; set; } = [];
        public int UnReadedCount { get; set; } = 0;
        public required ProductRequest ProductRequest { get; set; }
        public bool HasReview {  get; set; }
        public bool Blocked { get; set; }
        public required User ChatWithUser { get; set; }
        public DateTime CreatedAt { get; set; }        
    }
}
