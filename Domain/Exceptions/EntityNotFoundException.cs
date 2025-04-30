namespace Nonuso.Domain.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException() : base() { }

        public EntityNotFoundException(string message) : base(message) { }

        public EntityNotFoundException(string entityName, object id)
            : base($"{entityName} with id {id} was not found") { }
    }
}
