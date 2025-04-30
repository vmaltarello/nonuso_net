using Nonuso.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Nonuso.Domain.Entities
{
    public class Product : Entity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(255)]
        public required string Title { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(500)]
        public required string Description { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        public string? ImagesUrl { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public required string LocationName { get; set; }

        public bool IsEnabled { get; set; } = true;
        public int ViewCount { get; set; } = 0;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        public Category? Category { get; set; }
        public User? User { get; set; }
    }
}
