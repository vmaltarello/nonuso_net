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
        public Guid BlockerId { get; set; }

        [Required]
        public Guid BlockedId { get; set; }

        public UserBlockReason Reason { get; set; }

        public string? AdditionalInfo { get; set; }

        public Guid? ConversationId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("BlockerId")]
        public User? Blocker { get; set; }

        [ForeignKey("BlockedId")]
        public User? Blocked { get; set; }

        [ForeignKey("ConversationId")]
        public Conversation? Conversation { get; set; }
    }
}
