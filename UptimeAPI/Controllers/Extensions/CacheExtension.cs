using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UptimeAPI.Controllers.Extensions
{
    public static class CacheExtension 
    {
        public static void SetCache<T>(this IMemoryCache cache, string key, T value, int expireInMinutes = 2)
        {
            var options = new MemoryCacheEntryOptions();
            options.SetAbsoluteExpiration(TimeSpan.FromMinutes(expireInMinutes));
            cache.Set<T>(key, value, options);
        }

    }
}
