namespace Nonuso.Messages.Api
{
    public class ReviewModel
    {

    }

    public class ReviewParamModel
    {
        public required Guid ProductRequestId { get; set; }
        public required Guid ReviewedUserId { get; set; }
        public Guid ReviewerUserId { get; set; }
        public required int Stars { get; set; }
        public string? Content { get; set; }
    }

    public class ReviewResultModel
    {
        public required Guid Id { get; set; }
        public required int Stars { get; set; }
        public string? Content { get; set; }
        public required Guid ProductRequestId { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}
