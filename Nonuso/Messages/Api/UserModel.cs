using Nonuso.Messages.Api.Base;

namespace Nonuso.Messages.Api
{
    public class UserModel : IModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string UserName { get; set; } = null!;

    }

    public class  UserSignInParamModel
    {
        public required string UserName { get; set; }
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
        public string Token { get; set; } = null!;

    }
}
