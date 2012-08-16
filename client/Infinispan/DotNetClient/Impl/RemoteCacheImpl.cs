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
using Infinispan.DotnetClient;

namespace Infinispan.DotNetClient.Impl
{
    ///<summary>
    ///Concrete implementation of RemoteCache inteface
    ///Author: sunimalr@gmail.com
    ///</summary>
    public class RemoteCacheImpl<K, V> : IRemoteCache<K,V>
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
        public RemoteCacheImpl(RemoteCacheManager cacheManager, ClientConfig configuration, ISerializer s, TCPTransportFactory trans) :
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
        public RemoteCacheImpl(RemoteCacheManager cacheManager, ClientConfig configuration, String cacheName, ISerializer s, TCPTransportFactory trans) :
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
        public RemoteCacheImpl(RemoteCacheManager cacheManager, ClientConfig configuration, bool forceReturn, ISerializer s, TCPTransportFactory trans) :
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
        public RemoteCacheImpl(RemoteCacheManager cacheManager, ClientConfig configuration, String cacheName, bool forceReturn, ISerializer s, TCPTransportFactory trans)
        {
            this.config = configuration;
            this.serializer = s;
            this.codec = new Codec();
            this.config.ForceReturnValue = forceReturn;
            this.config.CacheName = cacheName;
            this.operationsFactory = new OperationsFactory(cacheName, config.TopologyId, forceReturn, this.codec);
            this.transportFactory = trans;
        }

        public int Size()
        {
            StatsOperation op = operationsFactory.NewStatsOperation();
            transport = transportFactory.getTransport();
            try
            {
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }
            return int.Parse(op.ExecuteOperation(this.transport)[ServerStatisticsTypes.CURRENT_NR_OF_ENTRIES]);
        }

        public bool IsEmpty()
        {
            return Size() == 0;
        }

        public IServerStatistics Stats()
        {
            StatsOperation op = operationsFactory.NewStatsOperation();
            transport = transportFactory.getTransport();
            try
            {
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }
            Dictionary<String, String> statsMap = (Dictionary<String, String>)op.ExecuteOperation(this.transport);
            IServerStatistics stats = new ServerStatisticsImpl();
            for (int i = 0; i < statsMap.Count; i++)
            {
                stats.AddStats(statsMap.ElementAt(i).Key, statsMap.ElementAt(i).Value);
            }
            return stats;
        }

        public V Put(K key, V val)
        {
            return Put(key, val, 0, 0);
        }

        public V Put(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis)
        {
            byte[] result=null;
            int lifespanSecs = TimeSpan.FromMilliseconds(lifespaninMillis).Seconds;
            int maxIdleSecs = TimeSpan.FromMilliseconds(maxIdleTimeinMillis).Seconds;
            PutOperation op = operationsFactory.NewPutKeyValueOperation(serializer.Serialize(key), serializer.Serialize(val), lifespanSecs, maxIdleSecs);
            transport = transportFactory.getTransport();
            try
            {
                result = (byte[])op.ExecuteOperation(transport);
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }

            if (result != null)
            {
                return (V)serializer.Deserialize(result);
            }
            else
            {
                return default(V);
            }

        }

        public V PutIfAbsent(K key, V val)
        {
            return PutIfAbsent(key, val, 0, 0);
        }

        public V PutIfAbsent(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis)
        {
            byte[] returnedValue = null;
            int lifespanSecs = TimeSpan.FromMilliseconds(lifespaninMillis).Seconds;
            int maxIdleSecs = TimeSpan.FromMilliseconds(maxIdleTimeinMillis).Seconds;
            PutIfAbsentOperation op = operationsFactory.NewPutIfAbsentOperation(serializer.Serialize(key), serializer.Serialize(val), lifespanSecs, maxIdleSecs);
            transport = transportFactory.getTransport();
            try
            {
                returnedValue = op.ExecuteOperation(transport);
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }
            return (V)serializer.Deserialize(returnedValue);
        }

        public V Replace(K key, V val)
        {
            return Replace(key, val, 0, 0);
        }

        public V Replace(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis)
        {
            int lifespanSecs = TimeSpan.FromMilliseconds(lifespaninMillis).Seconds;
            int maxIdleSecs = TimeSpan.FromMilliseconds(maxIdleTimeinMillis).Seconds;
            byte[] bytes;
            ReplaceOperation op = operationsFactory.NewReplaceOperation(serializer.Serialize(key), serializer.Serialize(val), lifespanSecs, maxIdleSecs);
            transport = transportFactory.getTransport();
            try
            {
                bytes = (byte[])op.ExecuteOperation(transport);
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }

            return (V)serializer.Deserialize(bytes);
        }

        public bool ContainsKey(K key)
        {
            ContainsKeyOperation op = operationsFactory.NewContainsKeyOperation(serializer.Serialize(key));
            bool res = false;
            transport = transportFactory.getTransport();
            try
            {
                res = (Boolean)op.ExecuteOperation(transport);
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }
            return res;
        }

        public V Get(K key)
        {
            byte[] keyBytes = serializer.Serialize(key);
            byte[] bytes = null;
            transport = transportFactory.getTransport();
            GetOperation op = operationsFactory.NewGetKeyOperation(keyBytes);
            try
            {
                bytes = (byte[])op.ExecuteOperation(transport);
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }

            V result = (V)serializer.Deserialize(bytes);
            return result;
        }

        public Dictionary<K, V> GetBulk(int size)
        {
            transport = transportFactory.getTransport();
            BulkGetOperation op = operationsFactory.NewBulkGetOperation(size);
            Dictionary<byte[], byte[]> result;
            try
            {
                result = op.ExecuteOperation(transport);
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }
            Dictionary<K, V> toReturn = new Dictionary<K, V>();
            for (int i = 0; i < result.Count; i++)
            {
                V val = (V)serializer.Deserialize(result.ElementAt(i).Value);
                K key = (K)serializer.Deserialize(result.ElementAt(i).Key);
                toReturn.Add(key, val);
            }
            return toReturn;
        }

        public Dictionary<K, V> GetBulk()
        {
            return GetBulk(0);
        }

        public V Remove(K key)
        {
            RemoveOperation removeOperation = operationsFactory.NewRemoveOperation(serializer.Serialize(key));
            transport = transportFactory.getTransport();
            byte[] existingValue;
            try
            {
                existingValue = (byte[])removeOperation.ExecuteOperation(transport);
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }

            return (V)serializer.Deserialize(existingValue);
        }

        public void Clear()
        {
            ClearOperation op = operationsFactory.NewClearOperation();
            transport = transportFactory.getTransport();
            try
            {
                op.ExecuteOperation(transport);
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }
        }
     
        public PingResult Ping()
        {
            PingResult res = PingResult.FAIL;
            transport = transportFactory.getTransport();
            try
            {
                res = operationsFactory.NewPingOperation(transport).Execute();
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }
            return res;
        }

        public IVersionedValue GetVersioned(K key)
        {
            IVersionedValue res = null;
            transport = transportFactory.getTransport();
            try
            {
                res = operationsFactory.NewGetWithVersionOperation(serializer.Serialize(key)).ExecuteOperation(transport);
            }
            finally
            {
                transportFactory.releaseTransport(transport);
            }
            return res;
        }

        public bool RemoveWithVersion(K key, long version)
        {
            VersionedOperationResponse res = null;
            transport = transportFactory.getTransport();
            try
            {
                res = operationsFactory.NewRemoveWithVersionOperation(serializer.Serialize(key), version).ExecuteOperation(transport);
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

        public bool ReplaceWithVersion(K key, V val, long version, int lifespaninMillis, int maxIdleTimeinMillis)
        {
            VersionedOperationResponse res = null;
            transport = transportFactory.getTransport();
            int lifespanSecs = TimeSpan.FromMilliseconds(lifespaninMillis).Seconds;
            int maxIdleSecs = TimeSpan.FromMilliseconds(maxIdleTimeinMillis).Seconds;
            try
            {
                res = operationsFactory.NewReplaceWithVersionOperation(serializer.Serialize(key), serializer.Serialize(val), lifespanSecs, maxIdleSecs, version).ExecuteOperation(transport);
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

        public bool ReplaceWithVersion(K key, V val, long version)
        {
            return ReplaceWithVersion(key, val, version, 0, 0);
        }

    }
}

