using Nonuso.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nonuso.Domain.Entities
{
    public enum ExchangeType
    {
        Unidirectional,
        Bidirectional
    }

    public enum ProductRequestStatus
    {
        Pending,
        Accepted,
        RejectedByOwner,
        RejectedBySystem,
        CancelledByRequester,
        Expired,
        ProductUnavailable
    }

    public class ProductRequest : Entity
    {
        [Required]
        public required Guid ProductId { get; set; }

        [Required]
        public required Guid RequesterId { get; set; }

        [Required]
        public required Guid RequestedId { get; set; }

        public Guid? ExchangeProductId { get; set; }

        [Required]
        public ExchangeType ExchangeType { get; set; } = ExchangeType.Unidirectional;

        [Required]
        public ProductRequestStatus Status { get; set; } = ProductRequestStatus.Pending;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("RequesterId")]
        public virtual User? RequesterUser { get; set; }
        
        [ForeignKey("RequestedId")]
        public virtual User? RequestedUser { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        [ForeignKey("ExchangeProductId")]
        public virtual Product? ExchangeProduct { get; set; }
    }
}