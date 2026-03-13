using Buildify.Core.Entities;

namespace Buildify.Core.Specifications
{
    public class OrdersBySellerSpecification : BaseSpecifications<Order>
    {
        public OrdersBySellerSpecification(string sellerId)
            : base(o => o.OrderItems.Any(oi => oi.Product.SellerId == sellerId))
        {
            AddInclude(o => o.OrderItems);
            AddInclude("OrderItems.Product");
            AddOrderByDescending(o => o.OrderDate);
        }
    }
}