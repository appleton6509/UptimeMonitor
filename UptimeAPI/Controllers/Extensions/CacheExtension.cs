using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace UptimeAPI.Controllers.Extensions
{
    public static class CacheExtension
    {
        public static void SetCache<T>(this IMemoryCache cache, string key, T value, int expireInSeconds = 9999)
        {
            var options = new MemoryCacheEntryOptions();
            options.SetAbsoluteExpiration(TimeSpan.FromMinutes(expireInSeconds));
            cache.Set<T>(key, value, options);
        }
        public static void Clear (this IMemoryCache cache)
        {
            PropertyInfo prop = cache.GetType().GetProperty("EntriesCollection", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
            object innerCache = prop.GetValue(cache);
            MethodInfo clearMethod = innerCache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
            clearMethod.Invoke(innerCache, null);
        }
    }
}
