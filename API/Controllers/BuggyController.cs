using API.Errors;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    
    public class BuggyController : BaseApiController
    {
        private readonly StoreContext storeContext;

        public BuggyController(StoreContext storeContext)
        {
            this.storeContext = storeContext;
        }
        [HttpGet("NotFound")]
        public ActionResult GetNotFoundRequest()
        {
            var product = storeContext.products.Find(42);
            if(product==null)
            {
                return NotFound(new ApiResponse(404));
            }
            return Ok();
        }
        [HttpGet("servererror")]
        public ActionResult GetServerError()
        {
            var product=storeContext.products.Find(42);
            var ProductToRturn=product.ToString();
            return Ok();
        }
         [HttpGet("badrequest")]
        public ActionResult GetBadRequest()
        {
            return BadRequest(new ApiResponse(400));
        }
        [HttpGet("badrequest/{id}")]
        public ActionResult GetBadRequest(int id)
        {
            return Ok();
        }
    }
}
