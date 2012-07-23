using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Util;
using Infinispan.DotNetClient.Trans.TCP;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Operations;
using Infinispan.DotNetClient;

namespace Infinispan.DotnetClient
{

    /**
         * 
         * Concrete implementation of RemoteCache inteface
         * Author: sunimalr@gmail.com
         * 
         */


    public class RemoteCacheImpl : RemoteCache
    {
        private ClientConfig config;
        private Serializer serializer;
        private Codec codec;
        private TCPTransportFactory transportFactory;
        private Transport transport;
        private OperationsFactory operationsFactory;

        /// <summary>
        /// The constructor of RemoteCacheImpl which uses default config properties.
        /// </summary>
        /// <param name="cacheManager">The CacheManager which holds the RemoteCache</param>
        /// <param name="configuration">Configuration of the client</param>
        /// <param name="s">Serializer to be used to. Pass a custom serializer of DefaultSerializer</param>
        /// <param name="start">Boolean start</param>
        public RemoteCacheImpl(RemoteCacheManager cacheManager, ClientConfig configuration, Serializer s, TCPTransportFactory trans) :
            this(cacheManager, configuration, "default", configuration.ForceReturnValue, s, trans)
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
        public RemoteCacheImpl(RemoteCacheManager cacheManager, ClientConfig configuration, String cacheName, Serializer s, TCPTransportFactory trans) :
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
        public RemoteCacheImpl(RemoteCacheManager cacheManager, ClientConfig configuration, bool forceReturn, Serializer s, TCPTransportFactory trans) :
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
        public RemoteCacheImpl(RemoteCacheManager cacheManager, ClientConfig configuration, String cacheName, bool forceReturn, Serializer s, TCPTransportFactory trans)
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
        public void put<V, K>(K key, V val)
        {
            put<V, K>(key, val, 0, 0);
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
        public void put<V, K>(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis)
        {
            int lifespanSecs = TimeSpan.FromMilliseconds(lifespaninMillis).Seconds;
            int maxIdleSecs = TimeSpan.FromMilliseconds(maxIdleTimeinMillis).Seconds;
            PutOperation op = operationsFactory.newPutKeyValueOperation(serializer.serialize(key), serializer.serialize(val), lifespanSecs, maxIdleSecs);
            transport = transportFactory.getTransport();
            try
            {
                byte[] result = (byte[])op.executeOperation(transport);
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }
        }


        /// <summary>
        /// Puts a new entry to cache only if the specified key is absent. Lifespan and maxidle time is infinite.
        /// </summary>
        /// <typeparam name="V">Data type of Value</typeparam>
        /// <typeparam name="K">Data type of Key</typeparam>
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        public void putIfAbsent<V, K>(K key, V val)
        {
            putIfAbsent<V, K>(key, val, 0, 0);
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
        public V putIfAbsent<V, K>(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis)
        {
            int lifespanSecs = TimeSpan.FromMilliseconds(lifespaninMillis).Seconds;
            int maxIdleSecs = TimeSpan.FromMilliseconds(maxIdleTimeinMillis).Seconds;
            PutIFAbsentOperation op = operationsFactory.newPutIfAbsentOperation(serializer.serialize(key), serializer.serialize(val), lifespanSecs, maxIdleSecs);
            transport = transportFactory.getTransport();
            try
            {
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }
            byte[] bytes = (byte[])op.executeOperation(transport);
            return (V)serializer.deserialize(bytes);
        }

        /// <summary>
        /// Replaces an existing value with a new value. Lifespan and maxidle time is infinite.
        /// </summary>
        /// <typeparam name="V">Data type of Value</typeparam>
        /// <typeparam name="K">Data type of Key</typeparam>
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        public V replace<V, K>(K key, V val)
        {
            return replace<V, K>(key, val, 0, 0);
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
        public V replace<V, K>(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis)
        {
            int lifespanSecs = TimeSpan.FromMilliseconds(lifespaninMillis).Seconds;
            int maxIdleSecs = TimeSpan.FromMilliseconds(maxIdleTimeinMillis).Seconds;
            ReplaceOperation op = operationsFactory.newReplaceOperation(serializer.serialize(key), serializer.serialize(val), lifespanSecs, maxIdleSecs);
            transport = transportFactory.getTransport();
            try
            {
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }
            byte[] bytes = (byte[])op.executeOperation(transport);
            return (V)serializer.deserialize(bytes);
        }

        /// <summary>
        /// Checks whether the passes key exists in the cache
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>True if the passed key exixts. False if not exist.</returns>
        public bool containsKey(Object key)
        {
            ContainsKeyOperation op = operationsFactory.newContainsKeyOperation(serializer.serialize(key));
            transport = transportFactory.getTransport();
            try
            {
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }
            return (Boolean)op.executeOperation(transport);
        }

        /// <summary>
        /// Used to retrieve the method 
        /// </summary>
        /// <typeparam name="V">Value</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Retrieved Value</returns>
        public V get<V>(Object key)
        {
            byte[] keyBytes = serializer.serialize(key);
            GetOperation op = operationsFactory.newGetKeyOperation(keyBytes);
            byte[] bytes = (byte[])op.executeOperation(transport);
            V result = (V)serializer.deserialize(bytes);
            return result;
        }

        /// <summary>
        /// Gets bulk data from the cache
        /// </summary>
        /// <typeparam name="K">Key</typeparam>
        /// <typeparam name="V">Value</typeparam>
        /// <param name="size">Number of records</param>
        /// <returns>Dictionary of retrieved data</returns>
        public Dictionary<K, V> getBulk<K, V>(int size)
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
        public Dictionary<K, V> getBulk<K, V>()
        {
            return getBulk<K, V>(0);
        }

        /// <summary>
        /// Removes a record with given key
        /// </summary>
        /// <typeparam name="K">Key Data Type</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Removed value</returns>
        public V remove<V>(Object key)
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
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }
            op.executeOperation(transport);
        }

        /// <summary>
        /// Application level operation to check existance of the cache
        /// </summary>
        /// <returns>PingRsult</returns>
        public PingOperation.PingResult ping()
        {
            transport = transportFactory.getTransport();
            try
            {
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }
            return operationsFactory.newPingOperation(transport).execute();
        }
    }
}

