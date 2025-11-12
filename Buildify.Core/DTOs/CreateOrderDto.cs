using System.ComponentModel.DataAnnotations;

namespace Buildify.Core.DTOs
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "Shipping address is required")]
        public ShippingAddressDto ShippingAddress { get; set; } = new ShippingAddressDto();
    }
}
