using Nonuso.Domain.Entities;

namespace Nonuso.Domain.Models
{
    public class ConversationModel
    {
        public Guid Id { get; set; }
        public required string LastMessage { get; set; }
        public DateTime LastMessageDate { get; set; }
        public required ProductRequest ProductRequest { get; set; }
        public required User ChatWithUser { get; set; }
        public DateTime CreatedAt { get; set; }
        
    }
}
