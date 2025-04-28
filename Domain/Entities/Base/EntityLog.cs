namespace Nonuso.Domain.Entities.Base
{
    public class EntityLog : Entity
    {
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsEnabled { get; set; }

        public void SetCreatedAt()
        {
            CreatedAt = DateTime.Now;
        }

        public void SetUpdatedAt()
        {
            UpdatedAt = DateTime.Now;
        }
    }
}
