namespace Nonuso.Api.Common
{
    public class CurrentUser(IHttpContextAccessor contextAccessor)
    {
        private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

        public Guid Id => Guid.Parse(_contextAccessor.HttpContext?.User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());
        public string Email => _contextAccessor.HttpContext?.User.FindFirst("email")?.Value ?? string.Empty;
        public string UserName => _contextAccessor.HttpContext?.User.FindFirst("nickname")?.Value ?? string.Empty;
    }
}
