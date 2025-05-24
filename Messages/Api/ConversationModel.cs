using Nonuso.Messages.Api.Base;

namespace Nonuso.Messages.Api
{
    public class MessageParamModel
    {
        public Guid ConversationId { get; set; }
        public Guid SenderId { get; set; }
        public required string Content { get;set; }
    }

    public class MessageResultModel
    {
        public Guid Id { get; set; }
        public bool IsMine { get; set; } = false;
        public required string Content { get; set; }
        public bool IsAttachment { get; set; } = false;
        public DateTime CreatedAt { get; set; }
    }

    public class ConversationModel : IModel
    {
        public Guid Id { get; set; }
        public Guid ProductRequestId { get; set; }
        public DateTime CreatedAt { get; set; }

    }

    public class ConversationResultModel
    {
        public Guid Id { get; set; }
        public required string LastMessage { get; set; }
        public DateTime LastMessageDate { get; set; }
        public IEnumerable<MessageResultModel> Messages { get; set; } = [];
        public required ProductRequestModel ProductRequest { get; set; }
        public required UserModel ChatWithUser { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ChatResultModel
    {
        public required ProductRequestModel ProductRequest { get; set; }
        public required UserModel ChatWithUser { get; set; }
        public IEnumerable<MessageResultModel> Messages { get; set; } = [];
    }
}