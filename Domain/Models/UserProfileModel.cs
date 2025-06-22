using Nonuso.Domain.Entities;

namespace Nonuso.Domain.Models
{
    public class UserProfileModel
    {
        public string UserName { get; set; } = null!;
        public IEnumerable<Review> Reviews { get; set; } = [];
    }
}
