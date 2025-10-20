using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class CacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly HashSet<string> _keys;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _keys = new HashSet<string>();
        }

        public void Set<T>(string key, T value, int CacheMinute)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheMinute),
                SlidingExpiration = TimeSpan.FromMinutes(30)
            };

            _memoryCache.Set(key, value, TimeSpan.FromMinutes(CacheMinute));
            _keys.Add(key);
        }


        public void Set<T>(string key, T value, TimeSpan CacheMinute)
        {

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheMinute,
                SlidingExpiration = TimeSpan.FromMinutes(30)
            };


            _memoryCache.Set(key, value, cacheEntryOptions);
            _keys.Add(key);
        }


        public bool TryGetValue<T>(string key, out T value)
        {
            return _memoryCache.TryGetValue(key, out value);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
            _keys.Remove(key);
        }

        public void RemoveByPrefix(string prefix)
        {
            try
            {
                if (_keys == null || _memoryCache == null || string.IsNullOrEmpty(prefix))
                {
                    return;
                }

                // Convert the prefix to lower case
                var lowerPrefix = prefix.ToLower();

                // Find keys that start with the lower case prefix
                var keysToRemove = _keys.Where(k => k.ToLower().StartsWith(lowerPrefix)).ToList();

                foreach (var key in keysToRemove)
                {
                    _memoryCache.Remove(key);
                    _keys.Remove(key);
                }

            }
            catch (Exception ex)
            {
                // Optionally log the exception or handle it appropriately
            }
        }


        public void RemoveBySuffix(string suffix)
        {
            try
            {
                if (_keys == null || _memoryCache == null || string.IsNullOrEmpty(suffix))
                {
                    return;
                }

                // Convert the suffix to lower case
                var lowerSuffix = suffix.ToLower();

                // Find keys that end with the lower case suffix
                var keysToRemove = _keys.Where(k => k.ToLower().EndsWith(lowerSuffix)).ToList();

                foreach (var key in keysToRemove)
                {
                    _memoryCache.Remove(key);
                    _keys.Remove(key);
                }
            }
            catch (Exception ex)
            {
                // Optionally log the exception or handle it appropriately
            }
        }



        public IEnumerable<string> GetAllKeys()
        {
            return _keys.ToList();
        }
    }
}
