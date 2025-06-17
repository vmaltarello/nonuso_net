using Nonuso.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Nonuso.Domain.Entities
{
    public class Category : Entity
    {
        [Required]
        [MinLength(5)]
        [MaxLength(255)]
        public string Description { get; set; } = null!;

        [Required]
        [MinLength(5)]
        [MaxLength(255)]
        public string DescriptionEN { get; set; } = null!;

    }
}