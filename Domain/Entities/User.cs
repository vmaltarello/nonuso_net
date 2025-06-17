using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Nonuso.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        [Required]
        public bool IsEnabled { get; set; } = true;

        public string? AvatarURL { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DeletedAt { get; set; }

        public DateTime? LastSignInAt { get; set; }
    }
}
