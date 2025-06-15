using Nonuso.Messages.Api.Base;

namespace Nonuso.Messages.Api
{
    public class UserModel : IModel
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public required string UserName { get; set; }

    }

    public class  UserChangePasswordParamModel 
    {
        public required string Password { get; set; }
    }

    public class  UserSignInParamModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class UserSignUpParamModel
    {
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }

    public class UserResultModel : UserModel
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public int RefreshTokenExpiresIn { get; set; }
    }
}
