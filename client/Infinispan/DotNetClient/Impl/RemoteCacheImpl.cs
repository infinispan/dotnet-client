using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Util;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Operations;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans.Impl.TCP;
using Infinispan.DotNetClient.Util.Impl;

namespace Infinispan.DotNetClient.Impl
{
    ///<summary>
    ///Concrete implementation of RemoteCache inteface
    ///Author: sunimalr@gmail.com
    ///</summary>
    public class RemoteCacheImpl<K, V> : RemoteCache<K,V>
    {
        private ClientConfig config;
        private ISerializer serializer;
        private Codec codec;
        private TCPTransportFactory transportFactory;
        private ITransport transport;
        private OperationsFactory operationsFactory;

        /// <summary>
        /// The constructor of RemoteCacheImpl which uses default config properties.
        /// </summary>
        /// <param name="cacheManager">The CacheManager which holds the RemoteCache</param>
        /// <param name="configuration">Configuration of the client</param>
        /// <param name="s">Serializer to be used to. Pass a custom serializer of DefaultSerializer</param>
        /// <param name="start">Boolean start</param>
        public RemoteCacheImpl(RemoteCacheManager<K, V> cacheManager, ClientConfig configuration, ISerializer s, TCPTransportFactory trans) :
            this(cacheManager, configuration, configuration.CacheName, configuration.ForceReturnValue, s, trans)
        {
        }

        /// <summary>
        /// The constructor of RemoteCacheImpl
        /// </summary>
        /// <param name="cacheManager">The CacheManager which holds the RemoteCache</param>
        /// <param name="configuration">Configuration of the client</param>
        /// <param name="cacheName">Pass the cachename if it differs from the default cache name</param>
        /// <param name="s">Serializer to be used to. Pass a custom serializer of DefaultSerializer</param>
        /// <param name="start">Boolean start</param>
        public RemoteCacheImpl(RemoteCacheManager<K, V> cacheManager, ClientConfig configuration, String cacheName, ISerializer s, TCPTransportFactory trans) :
            this(cacheManager, configuration, cacheName, configuration.ForceReturnValue, s, trans)
        {
        }

        /// <summary>
        ///  The constructor of RemoteCacheImpl
        /// </summary>
        /// <param name="cacheManager">The CacheManager which holds the RemoteCache</param>
        /// <param name="configuration">Configuration of the client</param>
        /// <param name="forceReturn">Pass ForceReturn value if it differs from the default falue</param>
        /// <param name="s">Serializer to be used to. Pass a custom serializer of DefaultSerializer</param>
        /// <param name="start">Boolean start</param>
        public RemoteCacheImpl(RemoteCacheManager<K, V> cacheManager, ClientConfig configuration, bool forceReturn, ISerializer s, TCPTransportFactory trans) :
            this(cacheManager, configuration, configuration.CacheName, forceReturn, s, trans)
        {
        }

        /// <summary>
        /// The constructor of RemoteCacheImpl
        /// </summary>
        /// <param name="cacheManager">The CacheManager which holds the RemoteCache</param>
        /// <param name="configuration">Configuration of the client</param>
        /// <param name="cacheName">Pass the cachename if it differs from the default cache name</param>
        /// <param name="forceReturn">Pass ForceReturn value if it differs from the default falue</param>
        /// <param name="s">Serializer to be used to. Pass a custom serializer of DefaultSerializer</param>
        /// <param name="start">Boolean start</param>
        public RemoteCacheImpl(RemoteCacheManager<K, V> cacheManager, ClientConfig configuration, String cacheName, bool forceReturn, ISerializer s, TCPTransportFactory trans)
        {
            this.config = configuration;
            this.serializer = s;
            this.codec = new Codec();
            this.config.ForceReturnValue = forceReturn;
            this.config.CacheName = cacheName;
            this.operationsFactory = new OperationsFactory(cacheName, config.TopologyId, forceReturn, this.codec);
            this.transportFactory = trans;
        }

        /// <summary>
        /// Used to get the number of records of the cache
        /// </summary>
        /// <returns>Current number of records in the cache</returns>
        public int size()
        {
            StatsOperation op = operationsFactory.newStatsOperation();
            transport = transportFactory.getTransport();
            try
            {
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }
            return int.Parse(op.executeOperation(this.transport)[ServerStatistics.CURRENT_NR_OF_ENTRIES]);
        }

        /// <summary>
        /// Used to check whether the cache is empty or not
        /// </summary>
        /// <returns>True if number of entries of the cache is zero</returns>
        public bool isEmpty()
        {
            return size() == 0;
        }

        /// <summary>
        /// Used to retreive statistical information about the remote cache
        /// </summary>
        /// <returns>Server Statistics</returns>
        public ServerStatistics stats()
        {
            StatsOperation op = operationsFactory.newStatsOperation();
            transport = transportFactory.getTransport();
            try
            {
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }
            Dictionary<String, String> statsMap = (Dictionary<String, String>)op.executeOperation(this.transport);
            ServerStatistics stats = new ServerStatistics();
            for (int i = 0; i < statsMap.Count; i++)
            {
                stats.addStats(statsMap.ElementAt(i).Key, statsMap.ElementAt(i).Value);
            }
            return stats;
        }

        /// <summary>
        /// Used to insert a new entry to the cache. Lifespan and maxidle time is infinite
        /// </summary>
        /// <typeparam name="V">Data type of Value</typeparam>
        /// <typeparam name="K">Data type of Key</typeparam>
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        public V put(K key, V val)
        {
            return put(key, val, 0, 0);
        }

        /// <summary>
        /// Used to insert a new entry to the cache
        /// </summary>
        /// <typeparam name="V">Data type of Value</typeparam>
        /// <typeparam name="K">Data type of Key</typeparam>
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        /// <param name="lifespaninMillis">Lifespan in milliseconds</param>
        /// <param name="maxIdleTimeinMillis">Maximum idle time in milliseconds</param>
        public V put(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis)
        {
            byte[] result=null;
            int lifespanSecs = TimeSpan.FromMilliseconds(lifespaninMillis).Seconds;
            int maxIdleSecs = TimeSpan.FromMilliseconds(maxIdleTimeinMillis).Seconds;
            PutOperation op = operationsFactory.newPutKeyValueOperation(serializer.serialize(key), serializer.serialize(val), lifespanSecs, maxIdleSecs);
            transport = transportFactory.getTransport();
            try
            {
                result = (byte[])op.executeOperation(transport);
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }

            if (result != null)
            {
                return (V)serializer.deserialize(result);
            }
            else
            {
                return default(V);
            }

        }


        /// <summary>
        /// Puts a new entry to cache only if the specified key is absent. Lifespan and maxidle time is infinite.
        /// </summary>
        /// <typeparam name="V">Data type of Value</typeparam>
        /// <typeparam name="K">Data type of Key</typeparam>
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        public bool putIfAbsent(K key, V val)
        {
            return putIfAbsent(key, val, 0, 0);
        }

        /// <summary>
        /// Puts a new entry to cache only if the specified key is absent.
        /// </summary>
        /// <typeparam name="V">Data type of Value</typeparam>
        /// <typeparam name="K">Data type of Key</typeparam>
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        /// <param name="lifespaninMillis">Lifespan in milliseconds</param>
        /// <param name="maxIdleTimeinMillis">Maximum idle time in milliseconds</param>
        public bool putIfAbsent(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis)
        {
            bool success = false;
            int lifespanSecs = TimeSpan.FromMilliseconds(lifespaninMillis).Seconds;
            int maxIdleSecs = TimeSpan.FromMilliseconds(maxIdleTimeinMillis).Seconds;
            PutIFAbsentOperation op = operationsFactory.newPutIfAbsentOperation(serializer.serialize(key), serializer.serialize(val), lifespanSecs, maxIdleSecs);
            transport = transportFactory.getTransport();
            try
            {
                success = op.executeOperation(transport);
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }
            return success;
        }

        /// <summary>
        /// Replaces an existing value with a new value. Lifespan and maxidle time is infinite.
        /// </summary>
        /// <typeparam name="V">Data type of Value</typeparam>
        /// <typeparam name="K">Data type of Key</typeparam>
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        public V replace(K key, V val)
        {
            return replace(key, val, 0, 0);
        }

        /// <summary>
        /// Replaces an existing value with a new value.
        /// </summary>
        /// <typeparam name="V">Data type of Value</typeparam>
        /// <typeparam name="K">Data type of Key</typeparam>
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        /// <param name="lifespaninMillis">Lifespan in milliseconds</param>
        /// <param name="maxIdleTimeinMillis">Maximum idle time in milliseconds</param>
        public V replace(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis)
        {
            int lifespanSecs = TimeSpan.FromMilliseconds(lifespaninMillis).Seconds;
            int maxIdleSecs = TimeSpan.FromMilliseconds(maxIdleTimeinMillis).Seconds;
            byte[] bytes;
            ReplaceOperation op = operationsFactory.newReplaceOperation(serializer.serialize(key), serializer.serialize(val), lifespanSecs, maxIdleSecs);
            transport = transportFactory.getTransport();
            try
            {
                bytes = (byte[])op.executeOperation(transport);
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }

            return (V)serializer.deserialize(bytes);
        }

        /// <summary>
        /// Checks whether the passes key exists in the cache
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>True if the passed key exixts. False if not exist.</returns>
        public bool containsKey(K key)
        {
            ContainsKeyOperation op = operationsFactory.newContainsKeyOperation(serializer.serialize(key));
            bool res = false;
            transport = transportFactory.getTransport();
            try
            {
                res = (Boolean)op.executeOperation(transport);
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }
            return res;
        }

        /// <summary>
        /// Used to retrieve a record from the cache with the specified key
        /// </summary>
        /// <typeparam name="V">Value</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Retrieved Value</returns>
        public V get(K key)
        {
            byte[] keyBytes = serializer.serialize(key);
            byte[] bytes = null;
            transport = transportFactory.getTransport();
            GetOperation op = operationsFactory.newGetKeyOperation(keyBytes);
            try
            {
                bytes = (byte[])op.executeOperation(transport);
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }

            V result = (V)serializer.deserialize(bytes);
            return result;
        }

        /// <summary>
        /// Gets more than one record at a time from the cache
        /// </summary>
        /// <typeparam name="K">Key</typeparam>
        /// <typeparam name="V">Value</typeparam>
        /// <param name="size">Number of records</param>
        /// <returns>Dictionary of retrieved data</returns>
        public Dictionary<K, V> getBulk(int size)
        {
            transport = transportFactory.getTransport();
            BulkGetOperation op = operationsFactory.newBulkGetOperation(size);
            Dictionary<byte[], byte[]> result;
            try
            {
                result = op.executeOperation(transport);
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }
            Dictionary<K, V> toReturn = new Dictionary<K, V>();
            for (int i = 0; i < result.Count; i++)
            {
                V val = (V)serializer.deserialize(result.ElementAt(i).Value);
                K key = (K)serializer.deserialize(result.ElementAt(i).Key);
                toReturn.Add(key, val);
            }
            return toReturn;
        }

        /// <summary>
        /// Gets bulk data from the cache
        /// </summary>
        /// <typeparam name="K">Key</typeparam>
        /// <typeparam name="V">Value</typeparam>
        /// <returns>Dictionary of retrieved data</returns>
        public Dictionary<K, V> getBulk()
        {
            return getBulk(0);
        }

        /// <summary>
        /// Removes a record with given key
        /// </summary>
        /// <typeparam name="K">Key Data Type</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Removed value</returns>
        public V remove(K key)
        {
            RemoveOperation removeOperation = operationsFactory.newRemoveOperation(serializer.serialize(key));
            transport = transportFactory.getTransport();
            byte[] existingValue;
            try
            {
                existingValue = (byte[])removeOperation.executeOperation(transport);
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }

            return (V)serializer.deserialize(existingValue);
        }

        /// <summary>
        /// Clears the cache
        /// </summary>
        public void clear()
        {
            ClearOperation op = operationsFactory.newClearOperation();
            transport = transportFactory.getTransport();
            try
            {
                op.executeOperation(transport);
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }
        }

        /// <summary>
        /// Application level operation to check existance of the cache
        /// </summary>
        /// <returns>PingResult</returns>
        public PingResult ping()
        {
            PingResult res = PingResult.FAIL;
            transport = transportFactory.getTransport();
            try
            {
                res = operationsFactory.newPingOperation(transport).execute();
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }
            return res;
        }

        /// <summary>
        /// Returns the VersionedValue associated to the supplied key param, or null if it doesn't exist.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>VersionedValue associated to the supplied key param, or null if it doesn't exist.</returns>
        public VersionedValue getVersioned(K key)
        {
            VersionedValue res = null;
            transport = transportFactory.getTransport();
            try
            {
                res = operationsFactory.newGetWithVersionOperation(serializer.serialize(key)).executeOperation(transport);
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }
            return res;
        }

        /// <summary>
        ///Removes the given value only if its version matches the supplied version.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="version">version numeric version that should match the one in the server for the operation to succeed</param>
        /// <param name="lifespaninMinMillis"></param>
        /// <returns>Version details of the removed entry</returns>
        public VersionedOperationResponse removeIfUnmodified(K key, long version)
        {
            VersionedOperationResponse res = null;
            transport = transportFactory.getTransport();
            try
            {
                res = operationsFactory.newRemoveIfUnmodifiedOperation(serializer.serialize(key), version).executeOperation(transport);
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }
            return res;
        }

        /// <summary>
        ///Replaces the given value only if its version matches the supplied version.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="version">version numeric version that should match the one in the server for the operation to succeed</param>
        /// <param name="lifespaninMillis"></param>
        /// <param name="maxIdleTimeinMillis"></param>
        /// <returns>true if the value has been replaced</returns>
        public bool replaceWithVersion(K key, V val, long version, int lifespaninMillis, int maxIdleTimeinMillis)
        {
            VersionedOperationResponse res = null;
            transport = transportFactory.getTransport();
            int lifespanSecs = TimeSpan.FromMilliseconds(lifespaninMillis).Seconds;
            int maxIdleSecs = TimeSpan.FromMilliseconds(maxIdleTimeinMillis).Seconds;
            try
            {
                res = operationsFactory.newReplaceWithVersionOperation(serializer.serialize(key), serializer.serialize(val), lifespanSecs, maxIdleSecs, version).executeOperation(transport);
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }

            if (res != null)
            {
                return res.GetCode().Equals(VersionedOperationResponse.RspCode.SUCCESS);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///Replaces the given value only if its version matches the supplied version.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="version">version numeric version that should match the one in the server for the operation to succeed</param>
        public bool replaceWithVersion(K key, V val, long version)
        {
            return replaceWithVersion(key, val, version, 0, 0);
        }

    }
}

