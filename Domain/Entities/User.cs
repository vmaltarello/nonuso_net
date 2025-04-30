using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Nonuso.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string AvatarURL { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastSignInAt { get; set; }
    }
}
