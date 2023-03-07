using API.Errors;
using System.Net;
using System.Text.Json;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> logger;
        private readonly IHostEnvironment env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, 
            IHostEnvironment env)
        {
            this.next = next;
            this.logger = logger;
            this.env = env;
        }
        //Middleware task 
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                //if there is no exception middleware moves to next request stage
                await next(httpContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,ex.Message);
                //writing our response to the context to send it to client
                httpContext.Response.ContentType = "application/json";
                //below line set the statuscode to 500 for internal server error
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                //Error response based on environmnt
                var response = env.IsDevelopment()
                    //if condition, for development environment adding stacktrace and message
                    ? new ApiException((int)HttpStatusCode.InternalServerError, ex.Message
                    , ex.StackTrace.ToString())
                    //for production environment
                    : new ApiException((int)HttpStatusCode.InternalServerError);

                var option =new JsonSerializerOptions{PropertyNamingPolicy=JsonNamingPolicy.CamelCase };

                var json = JsonSerializer.Serialize(response);
                 
                await httpContext.Response.WriteAsync(json);
            }
        }
    }
}
