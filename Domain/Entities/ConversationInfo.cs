using Nonuso.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nonuso.Domain.Entities
{
    public class ConversationInfo : Entity
    {
        [Required]
        public Guid ConversationId { get; set; }        

        [Required]
        public required Guid UserId { get; set; }      

        public bool Visible { get; set; } = true;

        public int UnreadCount { get; set; } = 0;

        [ForeignKey("ConversationId")]
        public virtual Conversation? Conversation { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
