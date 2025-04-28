using Microsoft.AspNetCore.Identity;

namespace Nonuso.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string AvatarURL { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastSignInAt { get; set; }
    }
}
