using System;
using System.IO;
using System.Runtime.Caching;

namespace XView
{
    /// <summary>
    /// Represents .NET runtime <see cref="MemoryCache.Default"/> cache.
    /// </summary>
    public class DefaultMemoryCache : ICache
    {
        private static readonly string cacheKeyPrefix = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());

        private static MemoryCache Cache
        {
            get { return MemoryCache.Default; }
        }

        /// <summary>
        /// Gets an entry from the cache for the given key.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <returns>Cached object or null.</returns>
        public object Get(string key)
        {
            return this.Contains(key) ? Cache.Get(MakeKeyUnique(key)) : null;
        }

        /// <summary>
        /// Sets given object with given key to cache. Cache lifetime is set to 15 minutes.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <param name="obj">Object to cache.</param>
        public void Set(string key, object obj)
        {
            var cacheItemPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(15.0) };
            this.Set(key, obj, cacheItemPolicy);
        }

        /// <summary>
        /// Determines whether a cache entry exists in the cache for the given key.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <returns>true/false</returns>
        public bool Contains(string key)
        {
            return Cache.Contains(MakeKeyUnique(key));
        }

        /// <summary>
        /// Removes a cache entry from cache.
        /// </summary>
        /// <param name="key">Cache key.</param>
        public void Remove(string key)
        {
            Cache.Remove(MakeKeyUnique(key));
        }

        /// <summary>
        /// Inserts a cache entry into the cache by using a key and a value and eviction. 
        /// </summary>
        /// <param name="key">Key associated with the data object.</param>
        /// <param name="obj">Object to cache.</param>
        /// <param name="policy"><see cref="CacheItemPolicy"/>.</param>
        public void Set(string key, object obj, CacheItemPolicy policy)
        {
            Cache.Set(MakeKeyUnique(key), obj, policy);
        }

        /// <summary>
        /// Gets a unique cache key for the given key. The returned cache key is unique to the 
        /// currently executing assembly in the current <see cref="AppDomain"/>.
        /// </summary>
        /// <param name="key">Cache key to make unique.</param>
        /// <returns>Unique cache key.</returns>
        private static string MakeKeyUnique(string key)
        {
            return string.Format("{0}|{1}", cacheKeyPrefix, key);
        }
    }
}