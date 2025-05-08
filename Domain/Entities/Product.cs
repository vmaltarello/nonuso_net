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
        public Guid UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public string ImagesUrl { get; set; } = string.Empty;

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        public Point Location { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string LocationName { get; set; } = string.Empty;

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
