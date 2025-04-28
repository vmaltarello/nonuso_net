using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Nonuso.Domain.Entities.Base;

namespace Nonuso.Domain.Entities
{
    public class Category : Entity
    {

        [Column("description")]
        [Required]
        public string Description { get; set; } = string.Empty;

        [Column("description_en")]
        [Required]
        public string DescriptionEN { get; set; } = string.Empty;

    }
}