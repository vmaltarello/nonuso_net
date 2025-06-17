using Nonuso.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nonuso.Domain.Entities
{
    public class LastSearch : Entity
    {
        [Required]
        public required Guid UserId { get; set; }

        [Required]
        [MaxLength(255)]
        public required string Search {  get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}