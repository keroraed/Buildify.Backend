using Buildify.Core.Entities;

namespace Buildify.Core.Specifications
{
    public class ProductWithCategorySpecification : BaseSpecifications<Product>
    {
        // Get all products with their categories
        public ProductWithCategorySpecification() : base()
        {
            AddInclude(p => p.Category);
            AddOrderBy(p => p.Name);
        }

        // Get a single product by ID with its category
        public ProductWithCategorySpecification(int id) : base(p => p.Id == id)
        {
            AddInclude(p => p.Category);
        }
    }
}
