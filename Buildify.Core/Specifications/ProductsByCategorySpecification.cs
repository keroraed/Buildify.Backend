using Buildify.Core.Entities;

namespace Buildify.Core.Specifications
{
    public class ProductsByCategorySpecification : BaseSpecifications<Product>
    {
        public ProductsByCategorySpecification(int categoryId) 
            : base(p => p.CategoryId == categoryId)
        {
            AddInclude(p => p.Category);
            AddOrderBy(p => p.Name);
        }
    }
}
