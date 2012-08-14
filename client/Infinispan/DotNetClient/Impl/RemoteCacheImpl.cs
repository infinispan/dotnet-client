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
using Infinispan.DotnetClient;

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

        public bool isEmpty()
        {
            return size() == 0;
        }

        public IServerStatistics stats()
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
            IServerStatistics stats = new ServerStatistics();
            for (int i = 0; i < statsMap.Count; i++)
            {
                stats.addStats(statsMap.ElementAt(i).Key, statsMap.ElementAt(i).Value);
            }
            return stats;
        }

        public V put(K key, V val)
        {
            return put(key, val, 0, 0);
        }

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

        public V putIfAbsent(K key, V val)
        {
            return putIfAbsent(key, val, 0, 0);
        }

        public V putIfAbsent(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis)
        {
            byte[] returnedValue = null;
            int lifespanSecs = TimeSpan.FromMilliseconds(lifespaninMillis).Seconds;
            int maxIdleSecs = TimeSpan.FromMilliseconds(maxIdleTimeinMillis).Seconds;
            PutIFAbsentOperation op = operationsFactory.newPutIfAbsentOperation(serializer.serialize(key), serializer.serialize(val), lifespanSecs, maxIdleSecs);
            transport = transportFactory.getTransport();
            try
            {
                returnedValue = op.executeOperation(transport);
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }
            return (V)serializer.deserialize(returnedValue);
        }

        public V replace(K key, V val)
        {
            return replace(key, val, 0, 0);
        }

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

        public Dictionary<K, V> getBulk()
        {
            return getBulk(0);
        }

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

        public IVersionedValue getVersioned(K key)
        {
            IVersionedValue res = null;
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

        public bool replaceWithVersion(K key, V val, long version)
        {
            return replaceWithVersion(key, val, version, 0, 0);
        }

    }
}

