using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buildify.Core.Entities
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // Shipping Address - stored as denormalized data
        [Required]
        [MaxLength(100)]
        public string ShippingFirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ShippingLastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string ShippingStreet { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ShippingCity { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ShippingState { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ShippingZipCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ShippingCountry { get; set; } = string.Empty;

        public DateTime? UpdatedDate { get; set; }

        // Navigation property
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
