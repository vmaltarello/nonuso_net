using Nonuso.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nonuso.Domain.Entities
{
    public class ProductHistory : Entity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public int TotalProductCount { get; set; } = 0;

        [Required]
        public int TotalExchangedCount { get; set; } = 0;

        [Required]
        public int TotalDonatedCount { get; set; } = 0;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
