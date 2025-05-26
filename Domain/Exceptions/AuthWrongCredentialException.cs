namespace Nonuso.Domain.Exceptions
{
    public class AuthWrongCredentialException : Exception
    {
        public AuthWrongCredentialException() : base() { }

        public AuthWrongCredentialException(string message) : base(message) { }
    }
}
