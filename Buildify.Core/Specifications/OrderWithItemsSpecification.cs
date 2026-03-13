using Buildify.Core.Entities;

namespace Buildify.Core.Specifications
{
    public class OrderWithItemsSpecification : BaseSpecifications<Order>
    {
        // Get all orders with items
        public OrderWithItemsSpecification() : base()
        {
            AddInclude(o => o.OrderItems);
            AddInclude("OrderItems.Product");
            AddOrderByDescending(o => o.OrderDate);
        }

        // Get single order by ID with items
        public OrderWithItemsSpecification(int orderId) : base(o => o.Id == orderId)
        {
            AddInclude(o => o.OrderItems);
            AddInclude("OrderItems.Product");
        }

        // Get orders by user ID with items
        public OrderWithItemsSpecification(string userId) : base(o => o.UserId == userId)
        {
            AddInclude(o => o.OrderItems);
            AddInclude("OrderItems.Product");
            AddOrderByDescending(o => o.OrderDate);
        }
    }
}
