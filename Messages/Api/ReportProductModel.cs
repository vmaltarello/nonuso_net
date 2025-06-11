namespace Nonuso.Messages.Api
{
    public class ReportProductModel
    {
    }

    public class ReportProductParamModel 
    {
        public required Guid ProductId { get; set; }
        public required ReportProductReasonModel Reason { get; set; }
        public required string Description { get; set; }
        public Guid UserId { get; set; }
    }

    public enum ReportProductReasonModel
    {
        WrongCategory,
        Duplicate,
        AlreadyTraded,
        NotOriginalItem,
        Bullying,
        ScamAttempt,
        ExplicitImages,
        AnimalWelfare,
        ProhibitedItem,
        OffensiveContent,
        DataFalsification,
        Other
    }
}
