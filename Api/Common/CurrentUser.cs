using Nonuso.Domain.Entities;
using System.Security.Claims;

namespace Nonuso.Api.Common
{
    public class CurrentUser(IHttpContextAccessor contextAccessor)
    {
        private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

        public Guid Id => Guid.Parse(_contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        public string Email => _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        public string UserName => _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
    }
}
