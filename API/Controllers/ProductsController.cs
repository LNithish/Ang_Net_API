using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Core.Specifications;
using API.DTOs;
using AutoMapper;
using API.Errors;
using API.Helpers;

namespace API.Controllers
{

    public class ProductsController : BaseApiController
    {
        //All th memory management are done by EF framework
        private readonly IProductRepository productRepository;

        //Generic services
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IGenericRepository<ProductType> _productTypesRepo;
        private readonly IGenericRepository<ProductBrand> _productBrandsRepo;

        //Automapper service
        private readonly IMapper mapper;

        public ProductsController(IProductRepository productRepository, 
            IGenericRepository<Product> productsRepo, IGenericRepository<ProductType> productTypesRepo,
            IGenericRepository<ProductBrand> productBrandsRepo,IMapper mapper)
        {
            this.productRepository = productRepository;
            _productsRepo = productsRepo;
            _productTypesRepo = productTypesRepo;
            _productBrandsRepo = productBrandsRepo;
            this.mapper=mapper;
        }

        //[HttpGet]
        ////adding sort for sorting
        ////setting up optional paramter brandId,brandtype for filtering
        ////public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts(string sort
        ////    ,int? brandId,int? typeId)
        ////adding productspecParam to avoid more functional parameters
        //public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts(
        //    //Telling API to check the parameters of ProductSpecParams in querystring
        //    [FromQuery]ProductSpecParams productSpecParams)
        //{
        //    //var products = await _productsRepo.ListAllAsync();

        ////Using Generic Repository with Specifications
        ////products sorting is passed, sort value is received from client
        //    var spec=new ProductsWithTypesAndBrandsSpecification(productSpecParams);
        //    var products = await _productsRepo.ListAsync(spec);
        //    //using DTO to transfer only required detail to client
        //    //return Ok(products);
        //    //var productsDto = products.Select(product => new ProductToReturnDto
        //    //{
        //    //    Id = product.Id,
        //    //    Name = product.Name,
        //    //    Description = product.Description,
        //    //    PictureUrl = product.PictureUrl,
        //    //    Price = product.Price,
        //    //    ProductBrand = product.ProductBrand.Name,
        //    //    ProductType = product.ProductType.Name
        //    //}).ToList();
        //    //return Ok(productsDto);

        //    //Replacing DTO with Automapper
        //    var productsMap=mapper.Map<IReadOnlyList<Product>,IReadOnlyList<ProductToReturnDto>>(products);
        //    return Ok(productsMap);
        //}


        [HttpGet]
       
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts(
           //Telling API to check the parameters of ProductSpecParams in querystring
           [FromQuery] ProductSpecParams productSpecParams)
        {
            //var products = await _productsRepo.ListAllAsync();

            //Using Generic Repository with Specifications
            //products sorting is passed, sort value is received from client
            var spec = new ProductsWithTypesAndBrandsSpecification(productSpecParams);
            var products = await _productsRepo.ListAsync(spec);
            //on mor spec for count of products
            var countspec = new ProductwithFiltersForCountSpecification(productSpecParams);
            var totalItems = await _productsRepo.CountAsync(countspec);

            //Replacing DTO with Automapper
            var paginatedItems= mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);
            return Ok(new Pagination<ProductToReturnDto>(productSpecParams.PageIndex,
                productSpecParams.PageSize,totalItems,paginatedItems));
        }



        [HttpGet("{id}")]
        //Swagger documentation
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            //var product = await _productsRepo.GetByIdAsync(id);

            //Using Generic Repository with Specifications
            var spec=new ProductsWithTypesAndBrandsSpecification(id);
            var product=await _productsRepo.GetEntityWithSpec(spec);

            //Error handling
            if(product==null)
            {
                return NotFound(new ApiResponse(404));
            }
            //using DTO to transfer only required detail to client 
            //var productDto = new ProductToReturnDto
            //{
            //    Id = product.Id,
            //    Name = product.Name,
            //    Description = product.Description,
            //    PictureUrl = product.PictureUrl,
            //    Price = product.Price,
            //    ProductBrand = product.ProductBrand.Name,
            //    ProductType = product.ProductType.Name
            //};
            //return productDto;

            //Replacing DTO with Automapper
            var productmap =mapper.Map<Product,ProductToReturnDto>(product);
            return productmap;
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
