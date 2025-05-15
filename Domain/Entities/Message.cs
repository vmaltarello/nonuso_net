using Nonuso.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nonuso.Domain.Entities
{
    public class Message : Entity
    {

        [Required]
        public Guid ConversationId { get; set; }
        
        [Required]
        public Guid SenderId { get; set; }

        [Required]
        public string? Content { get; set; }

        public bool IsAttachment { get; set; } = false;

        [Required]
        public bool DeletedForSender { get; set; } = false;

        [Required]
        public bool DeletedForReceiver { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ConversationId")]
        public Conversation? Conversation { get; set; }

        [ForeignKey("SenderId")]
        public User? SenderUser { get; set; }
    }
}
