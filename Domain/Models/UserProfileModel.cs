using Nonuso.Domain.Entities;

namespace Nonuso.Domain.Models
{
    public class UserProfileModel
    {
        public string UserName { get; set; } = null!;
        public IEnumerable<Review> Reviews { get; set; } = [];
        public required string JoinedMonth { get; set; }
        public required string JoinedYear { get; set; }
        public int ProductCount { get; set; } = 0;
    }
}
