using System.Collections.Generic;

namespace Infinispan.HotRod
{
    /// <summary>
    /// This class provides the user with methods for creating and removing remote cache
    /// </summary>
    public class RemoteCacheManagerAdmin
    {
        private RemoteCacheManager remoteCacheManager;
        private Infinispan.HotRod.SWIGGen.RemoteCacheManagerAdmin remoteCacheManagerAdmin;

        internal RemoteCacheManagerAdmin(RemoteCacheManager remoteCacheManager, Infinispan.HotRod.SWIGGen.RemoteCacheManagerAdmin remoteCacheManagerAdmin)
        {
            this.remoteCacheManager = remoteCacheManager;
            this.remoteCacheManagerAdmin = remoteCacheManagerAdmin;
        }

        /// <summary>
        /// Creates a cache on the container using the specified template.
        /// </summary>
        /// <typeparam name="K">class type for the cache key elements</typeparam>
        /// <typeparam name="V">class type for the cache value elements</typeparam>
        /// <param name="name">name of the cache</param>
        /// <param name="template">template configuration for the cache</param>
        /// <returns>the cache</returns>
        public IRemoteCache<K, V> CreateCache<K,V>(string name, string template)
        {
            remoteCacheManagerAdmin.createCache(name, template != null ? template : "" , "@@cache@create");
            return remoteCacheManager.GetCache<K, V>(name);
        }

        /// <summary>
        /// Retrieves an existing cache or creates one using the specified template if it doesn't exist
        /// </summary>
        /// <typeparam name="K">class type for the cache key elements</typeparam>
        /// <typeparam name="V">class type for the cache value elements</typeparam>
        /// <param name="name">name of the cache</param>
        /// <param name="template">template configuration for the cache</param>
        /// <returns>the cache</returns>
        public IRemoteCache<K, V> GetOrCreateCache<K, V>(string name, string template)
        {
            remoteCacheManagerAdmin.createCache(name, template != null ? template : "", "@@cache@getorcreate");
            return remoteCacheManager.GetCache<K, V>(name);
        }

        /// <summary>
        /// Creates a cache on the container using the specified XML configuration
        /// </summary>
        /// <typeparam name="K">class type for the cache key elements</typeparam>
        /// <typeparam name="V">class type for the cache value elements</typeparam>
        /// <param name="name">name of the cache</param>
        /// <param name="conf">XML configuration string for the cache</param>
        /// <returns></returns>
        public IRemoteCache<K, V> CreateCacheWithXml<K, V>(string name, string conf)
        {
            remoteCacheManagerAdmin.createCacheWithXml(name, conf, "@@cache@create");
            return remoteCacheManager.GetCache<K, V>(name);
        }

        /// <summary>
        /// Retrieves an existing cache or creates one using the specified XML configuration if it doesn't exist
        /// </summary>
        /// <typeparam name="K">class type for the cache key elements</typeparam>
        /// <typeparam name="V">class type for the cache value elements</typeparam>
        /// <param name="name">name of the cache</param>
        /// <param name="conf">XML configuration string for the cache</param>
        /// <returns></returns>
        public IRemoteCache<K, V> GetOrCreateCacheWithXml<K, V>(string name, string conf)
        {
            remoteCacheManagerAdmin.createCacheWithXml(name, conf, "@@cache@getorcreate");
            return remoteCacheManager.GetCache<K, V>(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public RemoteCacheManagerAdmin WithFlags(ISet<AdminFlag> flags)
        {
            remoteCacheManagerAdmin.withFlags(flags);
            return this;
        }
        /// <summary>
        /// Removes a cache from the remote server cluster.
        /// </summary>
        /// <param name="name">name of the cache</param>
        public void RemoveCache(string name)
        {
            remoteCacheManagerAdmin.removeCache(name);
        }

    }
}