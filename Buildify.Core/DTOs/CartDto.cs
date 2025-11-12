namespace Buildify.Core.DTOs
{
    public class CartDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public decimal TotalPrice => Items.Sum(item => item.Price * item.Quantity);
        public int TotalItems => Items.Sum(item => item.Quantity);
    }
}
