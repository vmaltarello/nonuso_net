using Nonuso.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Nonuso.Domain.Entities
{
    public class LastSearch : Entity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(255)]
        public string Search {  get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
    }
}