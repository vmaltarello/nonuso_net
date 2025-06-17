using Nonuso.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nonuso.Domain.Entities
{
    public class Review : Entity
    {
        [Required]
        public required Guid ReviewerUserId { get; set; }

        [Required]
        public required Guid ReviewedUserId { get; set; }

        [Required]
        public required Guid ProductRequestId { get; set; }

        [MaxLength(500)]
        public string? Content { get; set; }

        [Required]
        [Range(1,5)]
        public int Stars { get; set; } = 1;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ReviewerUserId")]
        public virtual User? ReviewerUser { get; set; }

        [ForeignKey("ReviewedUserId")]
        public virtual User? ReviewedUser { get; set; }

        [ForeignKey("ProductRequestId")]
        public virtual ProductRequest? ProductRequest { get; set; }
    }
}
