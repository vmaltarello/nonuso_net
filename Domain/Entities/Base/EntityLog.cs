namespace Nonuso.Domain.Entities.Base
{
    public class EntityLog : Entity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
        public bool IsEnabled { get; set; }
    }
}
