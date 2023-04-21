
namespace Core.Interfaces
{
    public interface IResponseCacheService
    {
        //every data cached will have a key, object type of cached data which will be returned to client
        //timspan for mentioning how long it should be in cache(Redis)
        Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive);

        Task<string> GetCachedResponseAsync(string cacheKey);
    }
}
