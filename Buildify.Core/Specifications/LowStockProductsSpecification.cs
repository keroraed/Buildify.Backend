using Buildify.Core.Entities;

namespace Buildify.Core.Specifications
{
    public class LowStockProductsSpecification : BaseSpecifications<Product>
    {
        public LowStockProductsSpecification() : base(p => p.Stock <= 10)
        {
            AddInclude(p => p.Category);
            AddOrderBy(p => p.Stock);
        }
    }
}
