using Nonuso.Messages.Api.Base;

namespace Nonuso.Messages.Api
{
    public enum ProductRequestStatusModel
    {
        Pending,
        Accepted,
        RejectedByOwner,
        RejectedBySystem,
        CancelledByRequester,
        Expired,
        ProductUnavailable
    }

    public class ProductRequestModel : IModel
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid RequesterId { get; set; }
        public Guid RequestedId { get; set; }
        public ProductRequestStatusModel Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ProductRequestParamModel
    {
        public Guid ProductId { get; set; }
        public Guid RequesterId { get; set; }
        public required string Message { get; set; }
    }

    public class ProductRequestResultModel : ProductRequestModel {}
}
