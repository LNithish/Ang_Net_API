using Infrastructure.Data;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        //All th memory management are done by EF framework
        private readonly StoreContext _storeContext;

        public ProductsController(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        //Asynchronous task allows other http requsts to process while the current task is running asynchronously
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts()
        {
            var products=await _storeContext.products.ToListAsync();
            return products;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _storeContext.products.FindAsync(id);
            return product;
        }
    }
}
