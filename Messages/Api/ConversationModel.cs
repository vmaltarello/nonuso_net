using Nonuso.Messages.Api.Base;

namespace Nonuso.Messages.Api
{
    public class ConversationModel : IModel
    {
        public Guid Id { get; set; }
        public Guid ProductRequestId { get; set; }
        public DateTime CreatedAt { get; set; }

    }

    public class ConversationResultModel : ConversationModel 
    {
        public required ProductResultModel Product { get; set; }
    }
}
