namespace Nonuso.Messages.Api
{
    public class RefreshTokenModel
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public int RefreshTokenExpiresIn { get; set; }
    }

    public class RefreshTokenParamModel
    {
        public required Guid UserId { get; set; }
        public required string RefreshToken { set; get; }
    }
}
