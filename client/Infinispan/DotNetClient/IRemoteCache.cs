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
    ///Provides remote reference to a Hot Rod server/cluster.
    ///<b>Concurrency</b>: implementors of this interface will support multi-threaded access.
    ///<b>Return values</b>: previously existing values for certain operations are not returned, null
    ///is returned instead. 
    ///<b>Synthetic operations</b>: aggregate operations are being implemented based on other Hot Rod operations. E.g. 
    ///GetBulk is implemented through multiple individual gets. This means that the
    ///these operations are not atomic and that they are costly since the number of network round-trips is not one
    ///<b>changing default behavior through Flags</b>: it is possible to change the
    ///default cache behaviour by using flags.
    ///IRemoteCache cache = getRemoteCache(true); //passing true will enable Force Return Values
    ///Object oldValue = cache.Put(aKey, aValue);
    ///In the previous example, using ForceRetunValue will make the client to
    ///also return previously existing value associated with "aKey". If this flag would not be present, Infinispan
    ///would return (by default) null. This is in order to avoid fetching a possibly large object from the remote
    ///server, which might not be needed. 
    ///<b><a href="http://community.jboss.org/wiki/Eviction">Eviction and expiration</a></b>: 
    ///Unlike local cache, which allows specifying time values with any granularity ,
    ///HotRod only supports seconds as time units. When using .NET Hotrod Client library, you can use milliseconds also.
    ///result in loss of precision for values specified as nanos or milliseconds.
    ///<br/> Another fundamental difference is in the case of lifespan (naturally does NOT apply for max idle): 
    ///If number of seconds is bigger than 30 days, this number of seconds is treated as UNIX time and so, 
    ///represents the number of seconds since 1/1/1970. Passing 0 as maxIdle and lifeSpan (which are also default values)
    ///these values are set to infinity<br/>
    ///
    /// Author: sunimalr@gmail.com
    /// </summary>
    /// <typeparam name="K">Data Tyepe of Key</typeparam>
    /// <typeparam name="V">Data Type of Value</typeparam>
    public interface IRemoteCache<K, V>
    {
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
        bool RemoveWithVersion(K key, long version);

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
    /// <summary>
    /// Ping operation can give one of the three results defined in the PingResult enum.
    /// </summary>
    public enum PingResult
    {
        /// <summary>
        ///Success if the ping request was responded correctly 
        /// </summary>
        SUCCESS,
        /// <summary>
        /// When the ping request fails due to non-existing cache
        /// </summary>
        CACHE_DOES_NOT_EXIST,
        /// <summary>
        ///For any other type of failures 
        /// </summary>
        FAIL,
    }
}
