using Nonuso.Domain.Entities;

namespace Nonuso.Domain.Models
{
    public class ChatModel
    {
        public required ProductRequest ProductRequest { get; set; }
        public required User ChatWithUser { get; set; }
        public IEnumerable<MessageModel> Messages { get; set; } = [];
    }
}
