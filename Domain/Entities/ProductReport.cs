using Nonuso.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nonuso.Domain.Entities
{
    public enum ReportProductReason 
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

    public enum ReportProductStatus
    {
        Pending,
        InProgress,
        Resolved,
        Rejected
    }

    public class ProductReport : Entity
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public ReportProductReason Reason { get; set; }

        [Required]
        public ReportProductStatus Status { get; set; } = ReportProductStatus.Pending;

        [Required]
        public required string Description { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public DateTime DateTime { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}