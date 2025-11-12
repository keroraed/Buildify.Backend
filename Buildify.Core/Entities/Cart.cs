using System.ComponentModel.DataAnnotations;

namespace Buildify.Core.Entities
{
    public class Cart
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        // Navigation property
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
