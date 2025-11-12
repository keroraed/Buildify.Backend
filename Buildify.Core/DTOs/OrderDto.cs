using Buildify.Core.Entities;

namespace Buildify.Core.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public ShippingAddressDto ShippingAddress { get; set; } = new ShippingAddressDto();
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
        public DateTime? UpdatedDate { get; set; }
    }
}
