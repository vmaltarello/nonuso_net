using Nonuso.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nonuso.Domain.Entities
{
    public class Conversation : Entity
    {
        [Required]
        public Guid ProductRequestId { get; set; }       

        public ICollection<Message> Messages { get; set; } = [];

        public ICollection<ConversationInfo> ConversationsInfo { get; set; } = [];

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ProductRequestId")]
        public virtual ProductRequest? ProductRequest { get; set; }
    }
}
