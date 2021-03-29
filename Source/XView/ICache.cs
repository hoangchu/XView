using System.Runtime.Caching;

namespace XView
{
    /// <summary>
    /// Caching interface.
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Gets the cache object associated with the given key.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <returns>Cache object.</returns>
        object Get(string key);

        /// <summary>
        /// Sets (add/update) a cache entry.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <param name="value">Cache object.</param>
        void Set(string key, object value);

        /// <summary>
        /// Sets (add/update) a cache entry with a given <see cref="CacheItemPolicy"/>.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <param name="value"><see cref="object"/>.</param>
        /// <param name="cacheItemPolicy"><see cref="CacheItemPolicy"/></param>
        void Set(string key, object value, CacheItemPolicy cacheItemPolicy);

        /// <summary>
        /// Checks whether a cache entry exists for the given key.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <returns>true or false.</returns>
        bool Contains(string key);

        /// <summary>
        /// Removes the cache entry.
        /// </summary>
        /// <param name="key">Cache key.</param>
        void Remove(string key);
    }
}