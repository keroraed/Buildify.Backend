using Buildify.Core.Entities;

namespace Buildify.Core.Specifications
{
    public class OrdersByUserSpecification : BaseSpecifications<Order>
    {
        public OrdersByUserSpecification(string userId) 
            : base(o => o.UserId == userId)
        {
            AddInclude(o => o.OrderItems);
            AddOrderByDescending(o => o.OrderDate);
        }
    }
}
