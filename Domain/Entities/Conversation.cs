using Nonuso.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Nonuso.Domain.Entities
{
    public class Conversation : Entity
    {
        [Required]
        public Guid ProductRequestId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Message> Messages { get; set; } = [];

        public ICollection<MessageInfo> MessageInfo { get; set; } = [];

        [ForeignKey("ProductRequestId")]
        public ProductRequest? ProductRequest { get; set; }
    }
}
