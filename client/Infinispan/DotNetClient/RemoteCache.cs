using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Operations;
using Infinispan.DotNetClient.Impl;

namespace Infinispan.DotNetClient
{
    public interface RemoteCache<K, V>
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
        int size();

        /// <summary>
        /// Used to check whether the cache is empty or not
        /// </summary>
        /// <returns>True if number of entries of the cache is zero</returns>
        bool isEmpty();

        /// <summary>
        /// Used to retreive statistical information about the remote cache defined in Infinispan.DotNetClient.Impl.ServerStatistics
        /// </summary>
        /// <returns>Server Statistics</returns>
        ServerStatistics stats();

        /// <summary>
        /// Used to insert a new entry to the cache
        /// </summary>
        /// <typeparam name="V">Data type of Value</typeparam>
        /// <typeparam name="K">Data type of Key</typeparam>
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        /// <param name="lifespaninMillis">Lifespan in milliseconds</param>
        /// <param name="maxIdleTimeinMillis">Maximum idle time in milliseconds</param>
        void put(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis);

        /// <summary>
        /// Used to insert a new entry to the cache. Lifespan and maxidle time is infinite
        /// </summary>
        /// <typeparam name="V">Data type of Value</typeparam>
        /// <typeparam name="K">Data type of Key</typeparam>
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        void put(K key, V val);

        /// <summary>
        /// Puts a new entry to cache only if the specified key is absent.
        /// </summary>
        /// <typeparam name="V">Data type of Value</typeparam>
        /// <typeparam name="K">Data type of Key</typeparam>
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        /// <param name="lifespaninMillis">Lifespan in milliseconds</param>
        /// <param name="maxIdleTimeinMillis">Maximum idle time in milliseconds</param>
        bool putIfAbsent(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis);

        /// <summary>
        /// Puts a new entry to cache only if the specified key is absent. Lifespan and maxidle time is infinite.
        /// </summary>
        /// <typeparam name="V">Data type of Value</typeparam>
        /// <typeparam name="K">Data type of Key</typeparam>
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        /// <returns>True of the operation was successful, else false</returns>
        bool putIfAbsent(K key, V val);

        /// <summary>
        /// Replaces an existing value with a new value.
        /// </summary>
        /// <typeparam name="V">Data type of Value</typeparam>
        /// <typeparam name="K">Data type of Key</typeparam>
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        /// <param name="lifespaninMillis">Lifespan in milliseconds</param>
        /// <param name="maxIdleTimeinMillis">Maximum idle time in milliseconds</param>
        V replace(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis);

        /// <summary>
        /// Replaces an existing value with a new value. Lifespan and maxidle time is infinite.
        /// </summary>
        /// <typeparam name="V">Data type of Value</typeparam>
        /// <typeparam name="K">Data type of Key</typeparam>
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        V replace(K key, V val);

        /// <summary>
        /// Checks whether the passes key exists in the cache
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>True if the passed key exixts. False if not exist.</returns>
        bool containsKey(K key);

        /// <summary>
        /// Used to retrieve a record from the cache with the specified key
        /// </summary>
        /// <typeparam name="V">Value</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Retrieved Value</returns>
        V get(K key);

        /// <summary>
        /// Gets more than one record at a time from the cache
        /// </summary>
        /// <typeparam name="K">Key</typeparam>
        /// <typeparam name="V">Value</typeparam>
        /// <param name="size">Number of records</param>
        /// <returns>Dictionary of retrieved data</returns>
        Dictionary<K, V> getBulk(int size);

        /// <summary>
        /// Gets more than one record at a time from the cache
        /// </summary>
        /// <typeparam name="K">Key</typeparam>
        /// <typeparam name="V">Value</typeparam>
        /// <returns>Dictionary of retrieved data</returns>
        Dictionary<K, V> getBulk();

        /// <summary>
        /// Removes a record with given key
        /// </summary>
        /// <typeparam name="K">Key Data Type</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Removed value</returns>
        V remove(K key);

        /// <summary>
        /// Clears the cache
        /// </summary>
        void clear();

        /// <summary>
        /// Application level operation to check existance of the cache
        /// </summary>
        /// <returns>PingRsult</returns>
        PingResult ping();

        /// <summary>
        /// Returns the VersionedValue associated to the supplied key param, or null if it doesn't exist.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>VersionedValue associated to the supplied key param, or null if it doesn't exist.</returns>
        VersionedValue getVersioned(K key);

        /// <summary>
        ///Removes the given value only if its version matches the supplied version.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="version">version numeric version that should match the one in the server for the operation to succeed</param>
        /// <param name="lifespaninMinMillis"></param>
        /// <returns>true if the value has been replaced</returns>
        VersionedOperationResponse removeIfUnmodified(K key, long version);

        /// <summary>
        ///Replaces the given value only if its version matches the supplied version.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="version">version numeric version that should match the one in the server for the operation to succeed</param>
        /// <param name="lifespaninMillis"></param>
        /// <param name="maxIdleTimeinMillis"></param>
        /// <returns>true if the value has been replaced</returns>
        bool replaceWithVersion(K key, V val, long version, int lifespaninMillis, int maxIdleTimeinMillis);

        /// <summary>
        ///Replaces the given value only if its version matches the supplied version.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="version">version numeric version that should match the one in the server for the operation to succeed</param>
        bool replaceWithVersion(K key, V val, long version);
    }
}
