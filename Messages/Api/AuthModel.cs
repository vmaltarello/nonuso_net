namespace Nonuso.Messages.Api
{

    public class AuthGoogleParamModel
    {
        public required string IdToken { get; set; }
    }

    public class AuthConfirmEmailParamModel
    {
        public required string Token { get; set; } 
        public required string Email { get; set; }
    }

    public class  RequestResetPasswordParamModel
    {
        public required string Email { get; set; }
    }

    public class ResetPasswordParamModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Token { get; set; }
    }
}
