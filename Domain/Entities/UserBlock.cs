using Nonuso.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nonuso.Domain.Entities
{
    public enum UserBlockReason
    {
        Scam,
        PromotionalMessage,
        DiscriminatoryMessage, 
        SexualHarassment, 
        AccountCompromisedOrDataTheft, 
        Other
    }
   
    public class UserBlock : Entity
    {
        [Required]
        public required Guid BlockerId { get; set; }

        [Required]
        public required Guid BlockedId { get; set; }

        public required UserBlockReason Reason { get; set; }

        public string? AdditionalInfo { get; set; }

        public Guid? ConversationId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("BlockerId")]
        public virtual User? Blocker { get; set; }

        [ForeignKey("BlockedId")]
        public virtual User? Blocked { get; set; }

        [ForeignKey("ConversationId")]
        public virtual Conversation? Conversation { get; set; }
    }
}
