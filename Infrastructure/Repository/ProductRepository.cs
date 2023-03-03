using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class ProductRepository : IProductRepository
    {
        //All th memory management are done by EF framework
        private readonly StoreContext _storeContext;

        public ProductRepository(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        //Asynchronous task allows other http requsts to process while the current task is running asynchronously

        public async Task<Product> GetProductByIdAsync(int id)
        {
            var product = await _storeContext.products
                //Eager loading of navigation properties to get brand,type values from foriegn key mapping
                .Include(p => p.ProductBrand)
                .Include(p => p.ProductType)
                //Include does not have definition for findasync
                //.FindAsync(id);
                .FirstOrDefaultAsync(p=> p.Id==id);
            return product;
        }

        public async Task<IReadOnlyList<Product>> GetProductsAsync()
        {
            var products = await _storeContext.products
                //Eager loading of navigation properties to get brand,type values from foriegn key mapping
                .Include(p=>p.ProductBrand)
                .Include(p=>p.ProductType)
                .ToListAsync();
            return products;
        }

        public async Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync()
        {
            var brands = await _storeContext.ProductBrands.ToListAsync();
            return brands;
        }

        public async Task<IReadOnlyList<ProductType>> GetProductTypesAsync()
        {
            var types=await _storeContext.ProductTypes.ToListAsync();
            return types;
        }
    }
}
