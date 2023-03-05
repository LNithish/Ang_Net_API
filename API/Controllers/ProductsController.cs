using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Core.Specifications;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        //All th memory management are done by EF framework
        private readonly IProductRepository productRepository;

        //Generic services
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IGenericRepository<ProductType> _productTypesRepo;
        private readonly IGenericRepository<ProductBrand> _productBrandsRepo;

        public ProductsController(IProductRepository productRepository, 
            IGenericRepository<Product> productsRepo, IGenericRepository<ProductType> productTypesRepo,
            IGenericRepository<ProductBrand> productBrandsRepo)
        {
            this.productRepository = productRepository;
            _productsRepo = productsRepo;
            _productTypesRepo = productTypesRepo;
            _productBrandsRepo = productBrandsRepo;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts()
        {
            //var products = await _productsRepo.ListAllAsync();

            //Using Generic Repository with Specifications
            var spec=new ProductsWithTypesAndBrandsSpecification();
            var products = await _productsRepo.ListAsync(spec);
            return Ok(products);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            //var product = await _productsRepo.GetByIdAsync(id);

            //Using Generic Repository with Specifications
            var spec=new ProductsWithTypesAndBrandsSpecification(id);
            var product=await _productsRepo.GetEntityWithSpec(spec);
            return product;
        }
        [HttpGet("brands")]
        public async Task<ActionResult<List<ProductBrand>>> GetProductBrands()
        {
            var brands= await _productBrandsRepo.ListAllAsync();
            //Ok response avoids list convrsion errors 
            return Ok(brands);
        }    
        [HttpGet("types")]
        public async Task<ActionResult<List<ProductType>>> GetProductTypes()
        {
            var types = await _productTypesRepo.ListAllAsync();
            //Ok response avoids list convrsion errors 
            return Ok(types);
        }
    }
}
