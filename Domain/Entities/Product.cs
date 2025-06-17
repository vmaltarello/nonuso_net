using NetTopologySuite.Geometries;
using Nonuso.Domain.Entities.Base;
using NpgsqlTypes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nonuso.Domain.Entities
{
    public class Product : Entity
    {

        [Required]
        public required Guid UserId { get; set; }

        [Required]
        [StringLength(255)]
        public required string Title { get; set; }

        [Required]
        [StringLength(500)]
        public required string Description { get; set; }

        public string? ImagesUrl { get; set; } = null;

        [Required]
        public required Guid CategoryId { get; set; }

        [Required]
        public required Point Location { get; set; }

        [Required]
        [StringLength(255)]
        public required string LocationName { get; set; }

        public NpgsqlTsVector SearchVector { get; set; } = null!;

        public int ViewCount { get; set; } = 0;

        public bool IsEnabled { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
