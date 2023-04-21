using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace API.Helpers
{
    //creating cache attribute
    //filter allows code to run before/after specific stages in request processing pipeline
    //so we can call the code before/After a Actionmethod is executed to cache data
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int timeToLiveSeconds;

        public CachedAttribute(int timeToLiveSeconds) 
        {
            this.timeToLiveSeconds = timeToLiveSeconds;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //before request it the controller below part executes
            //get reference of our cacheservice
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            //building a cache key based on querystring
            var cacheKey=GenerateCacheKeyFromRequest(context.HttpContext.Request);
            var cachedResponse=await cacheService.GetCachedResponseAsync(cacheKey);
            //if cachedresponse is not empty if executes and sends tha cached data to client
            if(!string.IsNullOrEmpty(cachedResponse)) 
            {
                //content response to send to client
                var contentResult = new ContentResult
                {
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result=contentResult;
                return;
            }

            var executedContext=await next();//move to controller and execute action method
            //saving result to cache for next time accessing it from cache
            if(executedContext.Result is OkObjectResult okObjectResult)
            {
                await cacheService.CacheResponseAsync(cacheKey,okObjectResult.Value
                    ,TimeSpan.FromSeconds(timeToLiveSeconds));   
            }
        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append($"{request.Path}");
            //loopin over all the values present in querystring
            foreach(var(key,value) in request.Query.OrderBy(x=>x.Key)) 
            {
                //| is used to separat each of query string values
                keyBuilder.Append($"|{key}-{value}");
            }
            return keyBuilder.ToString();
        }
    }
}
