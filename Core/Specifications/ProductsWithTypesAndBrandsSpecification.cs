using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class ProductsWithTypesAndBrandsSpecification : BaseSpecification<Product>
    {
        //? used for optional parameter
        //public ProductsWithTypesAndBrandsSpecification(string sort,int? brandId,int? typeID)
        public ProductsWithTypesAndBrandsSpecification(ProductSpecParams productSpecParams)
            //filtering based on product brand and type
            //passing brandID or typID to the constructor of basespecification to match criteria
            //search the product values
            : base(BasespecconstructorValu=>
            (string.IsNullOrEmpty(productSpecParams.Search)||BasespecconstructorValu.Name.ToLower()
                .Contains(productSpecParams.Search))&&
            (!productSpecParams.BrandId.HasValue||BasespecconstructorValu.ProductBrandId==productSpecParams.BrandId)&&
            (!productSpecParams.TypeId.HasValue||BasespecconstructorValu.ProductTypeId==productSpecParams.TypeId))
        {
            AddInclude(x => x.ProductType);
            AddInclude(x => x.ProductBrand);
            //Default sort based on product name
            AddOrderBy(x => x.Name);
            //5*(1-1)=0. so skip is 0 and sinc it is 00 we don't loose any data
            //pagesize will be 5, so take will be 5
            ApplyPaging(productSpecParams.PageSize * (productSpecParams.PageIndex - 1),
                productSpecParams.PageSize);

            if(productSpecParams.Sort != null)
            {
                switch (productSpecParams.Sort)
                {
                    case "priceAsc":
                        AddOrderBy(p => p.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDescending(p => p.Price);
                        break;
                    default:
                        AddOrderBy(p => p.Name);
                        break;
                }
            }
        }

        public ProductsWithTypesAndBrandsSpecification(int id) : base(x=>x.Id==id)
        {
            AddInclude(x => x.ProductType);
            AddInclude(x => x.ProductBrand);
        }
    }
}
