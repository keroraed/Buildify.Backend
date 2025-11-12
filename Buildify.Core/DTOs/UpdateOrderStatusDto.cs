using System.ComponentModel.DataAnnotations;
using Buildify.Core.Entities;

namespace Buildify.Core.DTOs
{
    public class UpdateOrderStatusDto
    {
        [Required(ErrorMessage = "Status is required")]
        public OrderStatus Status { get; set; }
    }
}
