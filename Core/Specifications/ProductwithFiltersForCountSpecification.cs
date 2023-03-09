using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class ProductwithFiltersForCountSpecification : BaseSpecification<Product>
    {
        public ProductwithFiltersForCountSpecification(ProductSpecParams productSpecParams) 
            : base(BasespecconstructorValu =>
            (string.IsNullOrEmpty(productSpecParams.Search) || BasespecconstructorValu.Name.ToLower()
                .Contains(productSpecParams.Search)) &&
            (!productSpecParams.BrandId.HasValue || BasespecconstructorValu.ProductBrandId == productSpecParams.BrandId) &&
            (!productSpecParams.TypeId.HasValue || BasespecconstructorValu.ProductTypeId == productSpecParams.TypeId))
        {

        }
    }
}
