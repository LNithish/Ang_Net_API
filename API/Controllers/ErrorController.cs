using API.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    //Overridding Route attriute from baseapicontroller
    [Route("errors/{code}")]
    public class ErrorController : BaseApiController
    {
        [HttpGet]
        public IActionResult Error(int code)
        {
            return new ObjectResult(new ApiResponse(code));
        }
    }
}
