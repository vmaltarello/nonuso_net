using Nonuso.Domain.Entities;

namespace Nonuso.Domain.Models
{
    public class ConversationModel : Conversation
    {
        public Product Product { get; set; } = null!;
    }
}
