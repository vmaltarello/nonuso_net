namespace Nonuso.Application.DTOs
{
    public class UserDto
    {
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastSignInAt { get; set; }
    }
}
