using Buildify.Core.Entities;

namespace Buildify.Core.Specifications
{
    public class ProductsBySellerSpecification : BaseSpecifications<Product>
    {
        // Get all products for a specific seller with their categories
        public ProductsBySellerSpecification(string sellerId) : base(p => p.SellerId == sellerId)
        {
            AddInclude(p => p.Category);
            AddOrderByDescending(p => p.CreatedDate);
        }
    }
}
