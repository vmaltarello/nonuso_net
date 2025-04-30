using Nonuso.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Nonuso.Domain.Entities
{
    public class Review : Entity
    {
        [Required]
        public required Guid ReviewerUserId { get; set; }

        [Required]
        public required Guid ReviewedUserId { get; set; }

        [MaxLength(500)]
        public string Content { get; set; } = string.Empty;

        [Required]
        [Range(1,5)]
        public int Stars { get; set; } = 0;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? ReviewerUser { get; set; }
        public User? ReviewedUser { get; set; }
    }
}
