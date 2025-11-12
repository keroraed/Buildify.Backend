using Buildify.Core.Entities;

namespace Buildify.Core.Specifications
{
    public class RecentOrdersSpecification : BaseSpecifications<Order>
    {
        public RecentOrdersSpecification() : base()
        {
            AddOrderByDescending(o => o.OrderDate);
            ApplyPaging(0, 10);
        }
    }
}
