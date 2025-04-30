using System.ComponentModel.DataAnnotations;

namespace Nonuso.Domain.Entities
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required Guid UserId { get; set; }

        [Required]
        [MaxLength(255)]
        public required string Token { get; set; }

        [Required]
        public required DateTime ExpirationDate { get; set; }

        public bool Revoked { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
    }
}
