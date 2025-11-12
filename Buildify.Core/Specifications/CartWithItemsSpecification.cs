using Buildify.Core.Entities;

namespace Buildify.Core.Specifications
{
    public class CartWithItemsSpecification : BaseSpecifications<Cart>
    {
        // Get cart by user ID with all items
        // Note: Product details need to be loaded separately via Include in repository
        public CartWithItemsSpecification(string userId) 
            : base(c => c.UserId == userId)
        {
            AddInclude(c => c.Items);
        }
    }
}
