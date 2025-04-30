using System.ComponentModel.DataAnnotations;

namespace Nonuso.Domain.Entities.Base
{
    public abstract class Entity
    {
        [Key]
        public Guid Id { get; set; }

        public void SetDefaults()
        {
            Id = Guid.NewGuid();
        }
    }
}
