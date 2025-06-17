using Nonuso.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nonuso.Domain.Entities
{
    public class Favorite : Entity
    {
        [Required]
        public required Guid ProductId { get; set; }

        [Required]
        public required Guid UserId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}