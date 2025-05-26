namespace Nonuso.Domain.Exceptions
{
    public class AuthUnauthorizedException : Exception
    {
        public AuthUnauthorizedException() : base() { }

        public AuthUnauthorizedException(string message) : base(message) { }
    }
}
