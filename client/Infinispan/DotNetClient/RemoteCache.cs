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


        int size();

        bool isEmpty();

        ServerStatistics stats();

        void put(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis);

        void put(K key, V val);

        bool putIfAbsent(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis);

        bool putIfAbsent(K key, V val);

        V replace(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis);

        V replace(K key, V val);

        bool containsKey(K key);

        V get(K key);

        Dictionary<K, V> getBulk(int size);

        Dictionary<K, V> getBulk();

        V remove(K key);

        void clear();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        PingResult ping();

        /// <summary>
        /// Returns the VersionedValue associated to the supplied key param, or null if it doesn't exist.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>VersionedValue associated to the supplied key param, or null if it doesn't exist.</returns>
        VersionedValue getVersioned(K key);


        VersionedOperationResponse removeIfUnmodified(K key, long version);

        /// <summary>
        /// /**
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
