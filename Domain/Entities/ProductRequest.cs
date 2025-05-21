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
        public Guid ProductId { get; set; }

        [Required]
        public Guid RequesterId { get; set; }

        [Required]
        public Guid RequestedId { get; set; }

        public Guid? ExchangeProductId { get; set; }

        [Required]
        public ExchangeType ExchangeType { get; set; } = ExchangeType.Unidirectional;

        [Required]
        public ProductRequestStatus Status { get; set; } = ProductRequestStatus.Pending;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("RequesterId")]
        public User? RequesterUser { get; set; }
        
        [ForeignKey("RequestedId")]
        public User? RequestedUser { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        [ForeignKey("ExchangeProductId")]
        public Product? ExchangeProduct { get; set; }
    }
}