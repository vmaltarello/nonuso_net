using Nonuso.Messages.Api.Base;

namespace Nonuso.Messages.Api
{
    public class UserBlockModel : IModel
    {
        public Guid Id { get; set; }
    }

    public class CheckUserBlockParamModel
    {
        public Guid CurrentUserId { get; set; }

        public required Guid OtherUserId { get; set; }

        public Guid? ConversationId { get; set; }
    }

    public class CheckUserBlockResultModel
    {
        public bool CurrentUserIsBlocked { get; set; }

        public bool OtherUserIsBlocked { get; set; }

        public Guid? ConversationId { get; set; }
    }

    public class UserBlockParamModel
    {
        public Guid BlockerId { get; set; }

        public required Guid BlockedId { get; set; }

        public required int Reason { get; set; }

        public string? AdditionalInfo { get; set; }

        public Guid? ConversationId { get; set; }
    }
}
