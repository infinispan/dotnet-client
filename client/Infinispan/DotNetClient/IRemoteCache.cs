using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Operations;
using Infinispan.DotNetClient.Impl;
using Infinispan.DotnetClient;

namespace Infinispan.DotNetClient
{
    /// <summary>
    /// IRemoteCache is the interface (API) through which Infinispan .NET client library users should call HotRod Operations. 
    /// 
    /// Author: sunimalr@gmail.com
    /// </summary>
    /// <typeparam name="K">Data Tyepe of Key</typeparam>
    /// <typeparam name="V">Data Type of Value</typeparam>
    public interface IRemoteCache<K, V>
    {

        /**
         * 
         * Remote Cache Interface
         * Author: sunimalr@gmail.com
         * 
         */

        /// <summary>
        /// Used to get the number of records of the cache
        /// </summary>
        /// <returns>Current number of records in the cache</returns>
        int Size();

        /// <summary>
        /// Used to check whether the cache is empty or not
        /// </summary>
        /// <returns>True if number of entries of the cache is zero</returns>
        bool IsEmpty();

        /// <summary>
        /// Used to retreive statistical information about the remote cache defined in Infinispan.DotNetClient.Impl.ServerStatistics
        /// </summary>
        /// <returns>Server Statistics</returns>
        IServerStatistics Stats();

        /// <summary>
        /// Used to insert a new entry to the cache
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        /// <param name="lifespaninMillis">Lifespan in milliseconds</param>
        /// <param name="maxIdleTimeinMillis">Maximum idle time in milliseconds</param>
        V Put(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis);

        /// <summary>
        /// Used to insert a new entry to the cache. Lifespan and maxidle time is infinite
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        V Put(K key, V val);

        /// <summary>
        /// Puts a new entry to cache only if the specified key is absent.
        /// </summary>
        /// <param name="key">key actual key to be added to the cache</param>
        /// <param name="val">value the value associated with the given key</param>
        /// <param name="lifespaninMillis">Lifespan in milliseconds</param>
        /// <param name="maxIdleTimeinMillis">Maximum idle time in milliseconds</param>
        V PutIfAbsent(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis);

        /// <summary>
        /// Puts a new entry to cache only if the specified key is absent. Lifespan and maxidle time is infinite.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        /// <returns>True of the operation was successful, else false</returns>
        V PutIfAbsent(K key, V val);

        /// <summary>
        /// Replaces an existing value with a new value.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        /// <param name="lifespaninMillis">Lifespan in milliseconds</param>
        /// <param name="maxIdleTimeinMillis">Maximum idle time in milliseconds</param>
        V Replace(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis);

        /// <summary>
        /// Replaces an existing value with a new value. Lifespan and maxidle time is infinite.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        V Replace(K key, V val);

        /// <summary>
        /// Checks whether the passes key exists in the cache
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>True if the passed key exixts. False if not exist.</returns>
        bool ContainsKey(K key);

        /// <summary>
        /// Used to retrieve a record from the cache with the specified key
        /// </summary>
        /// <typeparam name="V">Value</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Retrieved Value</returns>
        V Get(K key);

        /// <summary>
        /// Gets more than one record at a time from the cache
        /// </summary>
        /// <typeparam name="K">Key</typeparam>
        /// <typeparam name="V">Value</typeparam>
        /// <param name="size">Number of records</param>
        /// <returns>Dictionary of retrieved data</returns>
        Dictionary<K, V> GetBulk(int size);

        /// <summary>
        /// Gets more than one record at a time from the cache
        /// </summary>
        /// <typeparam name="K">Key</typeparam>
        /// <typeparam name="V">Value</typeparam>
        /// <returns>Dictionary of retrieved data</returns>
        Dictionary<K, V> GetBulk();

        /// <summary>
        /// Removes a record with given key
        /// </summary>
        /// <typeparam name="K">Key Data Type</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Removed value</returns>
        V Remove(K key);

        /// <summary>
        /// Clears the cache
        /// </summary>
        void Clear();

        /// <summary>
        /// Application level operation to check existance of the cache
        /// </summary>
        /// <returns>PingRsult</returns>
        PingResult Ping();

        /// <summary>
        /// Returns the VersionedValue associated to the supplied key param, or null if it doesn't exist.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>VersionedValue associated to the supplied key param, or null if it doesn't exist.</returns>
        IVersionedValue GetVersioned(K key);

        /// <summary>
        ///Removes the given value only if its version matches the supplied version.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="version">version numeric version that should match the one in the server for the operation to succeed</param>
        /// <param name="lifespaninMinMillis"></param>
        /// <returns>Version details of the removed entry</returns>
        VersionedOperationResponse RemoveIfUnmodified(K key, long version);

        /// <summary>
        ///Replaces the given value only if its version matches the supplied version.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="version">version numeric version that should match the one in the server for the operation to succeed</param>
        /// <param name="lifespaninMillis"></param>
        /// <param name="maxIdleTimeinMillis"></param>
        /// <returns>true if the value has been replaced</returns>
        bool ReplaceWithVersion(K key, V val, long version, int lifespaninMillis, int maxIdleTimeinMillis);

        /// <summary>
        ///Replaces the given value only if its version matches the supplied version.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="version">version numeric version that should match the one in the server for the operation to succeed</param>
        bool ReplaceWithVersion(K key, V val, long version);
    }
}
