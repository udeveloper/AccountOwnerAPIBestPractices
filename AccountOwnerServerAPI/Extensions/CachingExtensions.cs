using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountOwnerServerAPI.Extensions
{
    public static class CachingExtensions
    {
        public static T GetCache<T> (this object objectCache, string cacheKey, IDistributedCache cache)
        {
            string cachedJson = cache.GetString(cacheKey);            

            T objectCaching = default(T);

            if (!string.IsNullOrEmpty(cachedJson))
            {                
                objectCaching=  JsonConvert.DeserializeObject<T>(cachedJson);               
            }

            return objectCaching;
        }

        public static void SetCache<T>(this object objectCache, string cacheKey, IDistributedCache cache)
        {
            cache.SetString(cacheKey, JsonConvert.SerializeObject(objectCache),
                    new DistributedCacheEntryOptions() { AbsoluteExpiration = DateTime.Now.AddMinutes(1) });
        }
    }
}
