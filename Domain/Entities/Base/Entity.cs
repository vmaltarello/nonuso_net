namespace Nonuso.Domain.Entities.Base
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; }

        public void SetDefaults()
        {
            Id = Guid.NewGuid();
        }
    }
}
