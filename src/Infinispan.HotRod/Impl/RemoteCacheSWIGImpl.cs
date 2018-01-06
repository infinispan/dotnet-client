using System;
using System.Diagnostics;
using System.Collections.Generic;
using Infinispan.HotRod.SWIGGen;
using Infinispan.HotRod.Exceptions;
using Infinispan.HotRod.Event;
using Org.Infinispan.Query.Remote.Client;
using System.IO;
using Google.Protobuf;
using System.Threading.Tasks;
using System.Text;

namespace Infinispan.HotRod.Impl
{
#pragma warning disable 1591
    internal class RemoteCacheSWIGGenImpl<K, V> : IRemoteCache<K, V>
    {
        private RemoteByteArrayCache cache;
        private IMarshaller marshaller;
        private IMarshaller argMarshaller;
        private SWIG.RemoteCacheManager manager;
        private Infinispan.HotRod.Config.Configuration configuration;
        public RemoteCacheSWIGGenImpl(SWIG.RemoteCacheManager manager, Infinispan.HotRod.SWIG.RemoteByteArrayCache cache, IMarshaller marshaller, IMarshaller argMarshaller, Infinispan.HotRod.Config.Configuration configuration = null)
        {
            this.manager = manager;
            this.cache = (RemoteByteArrayCache)cache;
            this.marshaller = marshaller;
            this.argMarshaller = (argMarshaller != null) ? argMarshaller : new JBasicMarshaller();
            this.configuration = configuration;
        }

        public string GetName()
        {
            return cache.getName();
        }

        public string GetVersion()
        {
            return cache.getVersion();
        }

        public string GetProtocolVersion()
        {
            return cache.getProtocolVersion();
        }

        public ulong Size()
        {
            return cache.size();
        }

        public bool IsEmpty()
        {
            return cache.isEmpty();
        }

        public ServerStatistics Stats()
        {
            StringMap swigResult = cache.stats();
            if (swigResult == null)
            {
                return null;
            }
            return new ServerStatisticsImpl(unwrap(swigResult));
        }

        public V Put(K key, V val, ulong lifespan, TimeUnit lifespanUnit)
        {
            return (V)unwrap(cache.put(wrap(key), wrap(val), lifespan, wrap(lifespanUnit)));
        }

        public V Put(K key, V val, ulong lifespan, TimeUnit lifespanUnit, ulong maxIdleTime, TimeUnit maxIdleUnit)
        {
            return (V)unwrap(cache.put(wrap(key), wrap(val), lifespan, wrap(lifespanUnit), maxIdleTime, wrap(maxIdleUnit)));
        }

        public V Put(K key, V val)
        {
            return (V)unwrap(cache.put(wrap(key), wrap(val)));
        }

        public V PutIfAbsent(K key, V val, ulong lifespan, TimeUnit lifespanUnit, ulong maxIdleTime, TimeUnit maxIdleUnit)
        {
            return (V)unwrap(cache.putIfAbsent(wrap(key), wrap(val), lifespan, wrap(lifespanUnit), maxIdleTime, wrap(maxIdleUnit)));
        }

        public V PutIfAbsent(K key, V val, ulong lifespan, TimeUnit lifespanUnit)
        {
            return (V)unwrap(cache.putIfAbsent(wrap(key), wrap(val), lifespan, wrap(lifespanUnit)));
        }

        public V PutIfAbsent(K key, V val)
        {
            return (V)unwrap(cache.putIfAbsent(wrap(key), wrap(val)));
        }

        public void PutAll(IDictionary<K, V> map, ulong lifespan, TimeUnit lifespanUnit, ulong maxIdleTime, TimeUnit maxIdleUnit)
        {
            cache.putAll(wrap(map), lifespan, wrap(lifespanUnit), maxIdleTime, wrap(maxIdleUnit));
        }

        public void PutAll(IDictionary<K, V> map, ulong lifespan, TimeUnit lifespanUnit)
        {
            cache.putAll(wrap(map), lifespan, wrap(lifespanUnit));
        }

        public void PutAll(IDictionary<K, V> map)
        {
            cache.putAll(wrap(map));
        }

        public V Replace(K key, V val, ulong lifespan, TimeUnit lifespanUnit, ulong maxIdleTime, TimeUnit maxIdleUnit)
        {
            return (V)unwrap(cache.replace(wrap(key), wrap(val), lifespan, wrap(lifespanUnit), maxIdleTime, wrap(maxIdleUnit)));
        }

        public V Replace(K key, V val, ulong lifespan, TimeUnit lifespanUnit)
        {
            return (V)unwrap(cache.replace(wrap(key), wrap(val), lifespan, wrap(lifespanUnit)));
        }

        public V Replace(K key, V val)
        {
            return (V)unwrap(cache.replace(wrap(key), wrap(val)));
        }

        public bool ContainsKey(K key)
        {
            return cache.containsKey(wrap(key));
        }

        public bool ContainsValue(V val)
        {
            return cache.containsValue(wrap(val));
        }

        public V Get(K key)
        {
            return (V)unwrap(cache.get(wrap(key)));
        }

        public IDictionary<K, V> GetBulk(int size)
        {
            return unwrap(cache.getBulk(size));
        }

        public IDictionary<K,V> GetAll(ISet<K> keySet)
        {
            ByteArrayVector v = new ByteArrayVector();
            foreach (var k in keySet)
            {
                v.Add(wrap(k));
            }
            return unwrap (cache.getAll(v));
        }

        public IDictionary<K, V> GetBulk()
        {
            return unwrap(cache.getBulk());
        }

        public V Remove(K key)
        {
            return (V)unwrap(cache.remove(wrap(key)));
        }

        public void Clear()
        {
            cache.clear();
        }

        public IVersionedValue<V> GetVersioned(K key)
        {
            ValueVersionPair pair = cache.getWithVersion(wrap(key));
            if (pair == null || pair.first == null)
            {
                return null;
            }
            return new VersionedValueImpl<V>((V)unwrap(pair.first),
                                             (ulong)pair.second.version);
        }

        public IMetadataValue<V> GetWithMetadata(K key)
        {
            ValueMetadataPair pair = cache.getWithMetadata(wrap(key));
            if (pair == null || pair.first == null)
            {
                return null;
            }
            return new MetadataValueImpl<V>((V)unwrap(pair.first),
                                            (ulong)pair.second.version,
                                            pair.second.created,
                                            pair.second.lastUsed,
                                            pair.second.lifespan,
                                            pair.second.maxIdle);
        }

        public bool RemoveWithVersion(K key, ulong version)
        {
            return cache.removeWithVersion(wrap(key), version);
        }

        public bool ReplaceWithVersion(K key, V val, ulong version, ulong lifespan, ulong maxIdleTime)
        {
            return cache.replaceWithVersion(wrap(key), wrap(val), version, lifespan, maxIdleTime);
        }

        public bool ReplaceWithVersion(K key, V val, ulong version, ulong lifespan)
        {
            return cache.replaceWithVersion(wrap(key), wrap(val), version, lifespan);
        }

        public bool ReplaceWithVersion(K key, V val, ulong version)
        {
            return cache.replaceWithVersion(wrap(key), wrap(val), version);
        }

        public ISet<KeyValuePair<K, V>> EntrySet()
        {
            throw new Infinispan.HotRod.Exceptions.UnsupportedOperationException("Unsupported operation.");
        }

        public ISet<K> KeySet()
        {
            ByteArrayVector swigResult = hotrodcs.as_vector(cache.keySet());
            HashSet<K> result = new HashSet<K>();
            if (swigResult != null) {
                foreach (ByteArray item in swigResult) {
                    result.Add((K)unwrap(item));
                }
            }
            return (ISet<K>)result;
        }

        public IList<V> Values()
        {
            throw new Infinispan.HotRod.Exceptions.UnsupportedOperationException("Unsupported operation.");
        }

        public IRemoteCache<K, V> WithFlags(Flags flags)
        {
            cache.withFlags((Infinispan.HotRod.SWIGGen.Flag)flags);
            return this;
        }


        public QueryResponse Query(QueryRequest qr)
        {
            uint size = (uint)qr.CalculateSize();
            byte[] bytes = new byte[size];
            CodedOutputStream cos = new CodedOutputStream(bytes);
            qr.WriteTo(cos);
            cos.Flush();
            string s2 = System.Text.Encoding.ASCII.GetString(bytes);
            VectorByte vb = new VectorByte(bytes);
            VectorByte resp = cache.query(vb, size);
            byte[] respBytes = new byte[resp.Count];
            resp.CopyTo(respBytes);
            QueryResponse queryResp = new QueryResponse();
            return QueryResponse.Parser.ParseFrom(respBytes);
        }

        public object Execute(string scriptName, IDictionary<string, Object> dict = null)
        {
            VectorMap vm = new VectorMap();
            if (dict != null)
            {
                foreach (KeyValuePair<string, Object> p in dict)
                {
                    VectorChar vcKey = toVectorChar(p.Key);
                    VectorChar vcValue = toVectorChar(argMarshaller.ObjectToByteBuffer(p.Value));
                    vm.Add(vcKey, vcValue);
                }
            }
            VectorByte vb = cache.execute(scriptName, vm);
            byte[] ret = new byte[vb.Count];
            vb.CopyTo(ret);
            return argMarshaller.ObjectFromByteBuffer(ret);
        }

        public Task<V> GetAsync(K key)
        {
            if (!manager.IsStarted())
            {
                String message = "Cannot perform operations on a cache associated with an unstarted RemoteCacheManager. Use RemoteCacheManager.start before using the remote cache.";
                throw new Infinispan.HotRod.Exceptions.RemoteCacheManagerNotStartedException(message);
            }
            else
            {
                return Task.Run(() => Get(key));
            }
        }

        public Task<V> PutAsync(K key, V val, ulong lifespan = 0, TimeUnit lifespanUnit = TimeUnit.MILLISECONDS, ulong maxIdleTime = 0, TimeUnit maxIdleUnit = TimeUnit.MILLISECONDS)
        {
            if (!manager.IsStarted())
            {
                String message = "Cannot perform operations on a cache associated with an unstarted RemoteCacheManager. Use RemoteCacheManager.start before using the remote cache.";
                throw new Infinispan.HotRod.Exceptions.RemoteCacheManagerNotStartedException(message);
            }
            else
            {
                return Task.Run(() => Put(key, val, lifespan, lifespanUnit, maxIdleTime, maxIdleUnit));
            }
        }

        public Task PutAllAsync(IDictionary<K, V> map, ulong lifespan = 0, TimeUnit lifespanUnit = TimeUnit.MILLISECONDS, ulong maxIdleTime = 0, TimeUnit maxIdleUnit = TimeUnit.MILLISECONDS)
        {
            if (!manager.IsStarted())
            {
                String message = "Cannot perform operations on a cache associated with an unstarted RemoteCacheManager. Use RemoteCacheManager.start before using the remote cache.";
                throw new Infinispan.HotRod.Exceptions.RemoteCacheManagerNotStartedException(message);
            }
            else
            {
                return Task.Run(() => PutAll(map, lifespan, lifespanUnit, maxIdleTime, maxIdleUnit));
            }
        }

        public Task<V> PutIfAbsentAsync(K key, V val, ulong lifespan = 0, TimeUnit lifespanUnit = TimeUnit.MILLISECONDS, ulong maxIdleTime = 0, TimeUnit maxIdleUnit = TimeUnit.MILLISECONDS)
        {
            if (!manager.IsStarted())
            {
                String message = "Cannot perform operations on a cache associated with an unstarted RemoteCacheManager. Use RemoteCacheManager.start before using the remote cache.";
                throw new Infinispan.HotRod.Exceptions.RemoteCacheManagerNotStartedException(message);
            }
            else
            {
                return Task.Run(() => PutIfAbsent(key, val, lifespan, lifespanUnit, maxIdleTime, maxIdleUnit));
            }
        }

        public Task<V> ReplaceAsync(K key, V val, ulong lifespan = 0, TimeUnit lifespanUnit = TimeUnit.MILLISECONDS, ulong maxIdleTime = 0, TimeUnit maxIdleUnit = TimeUnit.MILLISECONDS)
        {
            if (!manager.IsStarted())
            {
                String message = "Cannot perform operations on a cache associated with an unstarted RemoteCacheManager. Use RemoteCacheManager.start before using the remote cache.";
                throw new Infinispan.HotRod.Exceptions.RemoteCacheManagerNotStartedException(message);
            }
            else
            {
                return Task.Run(() => Replace(key, val, lifespan, lifespanUnit, maxIdleTime, maxIdleUnit));
            }
        }

        public Task<V> RemoveAsync(K key)
        {
            if (!manager.IsStarted())
            {
                String message = "Cannot perform operations on a cache associated with an unstarted RemoteCacheManager. Use RemoteCacheManager.start before using the remote cache.";
                throw new Infinispan.HotRod.Exceptions.RemoteCacheManagerNotStartedException(message);
            }
            else
            {
                return Task.Run(() => Remove(key));
            }
        }

        public Task ClearAsync()
        {
            if (!manager.IsStarted())
            {
                String message = "Cannot perform operations on a cache associated with an unstarted RemoteCacheManager. Use RemoteCacheManager.start before using the remote cache.";
                throw new Infinispan.HotRod.Exceptions.RemoteCacheManagerNotStartedException(message);
            }
            else
            {
               return Task.Run(() => Clear());
            }
        }

        public Task<bool> ReplaceWithVersionAsync(K key, V val, ulong version, ulong lifespan = 0, ulong maxIdleTime = 0)
        {
            if (!manager.IsStarted())
            {
                String message = "Cannot perform operations on a cache associated with an unstarted RemoteCacheManager. Use RemoteCacheManager.start before using the remote cache.";
                throw new Infinispan.HotRod.Exceptions.RemoteCacheManagerNotStartedException(message);
            }
            return Task.Run(() => ReplaceWithVersion(key, val, version, lifespan, maxIdleTime));
        }

    public Task<bool> RemoveWithVersionAsync(K key, ulong version)
        {
            if (!manager.IsStarted())
            {
                String message = "Cannot perform operations on a cache associated with an unstarted RemoteCacheManager. Use RemoteCacheManager.start before using the remote cache.";
                throw new Infinispan.HotRod.Exceptions.RemoteCacheManagerNotStartedException(message);
            }
            else
            {
                return Task.Run(() => RemoveWithVersion(key, version));
            }
        }

        VectorChar toVectorChar(string s)
        {
            if (s == null) return null;
            VectorChar v = new VectorChar();
            foreach (char c in s)
            {
                v.Add(c);
            }
            return v;
        }

        VectorChar toVectorChar(byte[] s)
        {
            if (s == null) return null;
            VectorChar v = new VectorChar();
            foreach (char c in s)
            {
                v.Add(c);
            }
            return v;
        }

        string toString(VectorChar v)
        {
            char[] cc = new char[v.Count];
            int i = 0;
            foreach (char c in v)
            {
                cc.SetValue((char)(c & 0x00ff), i++);
            }
            return new string(cc);
        }

        byte[] toByteArray(VectorChar v)
        {
            byte[] bytes = new byte[v.Count];
            int i = 0;
            foreach (char c in v)
            {
                bytes[i++] = (byte)c;
            }
            return bytes;
        }

        byte[] toByteArray(VectorByte v)
        {
            byte[] bytes = new byte[v.Count];
            int i = 0;
            foreach (byte b in v)
            {
                bytes[i++] = b;
            }
            return bytes;
        }

        public void AddClientListener(Event.ClientListener<K,V> cl, string[] filterFactoryParams, string[] converterFactoryParams, Action recoveryCallback)
        {
            VectorVectorChar vvcFilterParams = new VectorVectorChar();
            foreach (string s in filterFactoryParams)
            {
                vvcFilterParams.Add(toVectorChar(marshaller.ObjectToByteBuffer(s)));
            }

            VectorVectorChar vvcConverterParams = new VectorVectorChar();
            foreach (string s in converterFactoryParams)
            {
                vvcConverterParams.Add(toVectorChar(marshaller.ObjectToByteBuffer(s)));
            }
            InternalClientEventListenerCallback<K,V> cb = new InternalClientEventListenerCallback<K,V>(cl, marshaller);
            DotNetClientListener listener =cache.addClientListener(cb, toVectorChar(cl.filterFactoryName), toVectorChar(cl.converterFactoryName), cl.includeCurrentState, vvcFilterParams, vvcConverterParams, cl.useRawData, cl.interestFlag);
            cl.listenerId = toByteArray(listener.getUListenerId());
            addListener(cl.listenerId, listener);
        }

        private void AddClientListener<CQK,CQV>(Event.ClientListener<CQK, CQV> cl, string[] filterFactoryParams, string[] converterFactoryParams, Action recoveryCallback)
        {
            VectorVectorChar vvcFilterParams = new VectorVectorChar();
            foreach (string s in filterFactoryParams)
            {
                vvcFilterParams.Add(toVectorChar(marshaller.ObjectToByteBuffer(s)));
            }

            VectorVectorChar vvcConverterParams = new VectorVectorChar();
            foreach (string s in converterFactoryParams)
            {
                vvcConverterParams.Add(toVectorChar(marshaller.ObjectToByteBuffer(s)));
            }
            InternalClientEventListenerCallback<CQK, CQV> cb = new InternalClientEventListenerCallback<CQK, CQV>(cl, marshaller);
            DotNetClientListener listener = cache.addClientListener(cb, toVectorChar(cl.filterFactoryName), toVectorChar(cl.converterFactoryName), cl.includeCurrentState, vvcFilterParams, vvcConverterParams, cl.useRawData, cl.interestFlag);
            cl.listenerId = toByteArray(listener.getUListenerId());
            addListener(cl.listenerId, listener);
        }

        public void AddContinuousQueryListener<CQK,CQV>(Event.ContinuousQueryListener<CQK, CQV> cql)
        {
            cql.clientEventListener = new ClientListener<CQK, CQV>();
            cql.clientEventListener.filterFactoryName = "continuous-query-filter-converter-factory";
            cql.clientEventListener.converterFactoryName = "continuous-query-filter-converter-factory";
            cql.clientEventListener.useRawData = true;
            cql.marshaller = this.marshaller;
            Action<ClientCacheEntryCustomEvent> f = (ClientCacheEntryCustomEvent ev) => { cql.ContinuousQueryListenerFunction(ev); };
            cql.clientEventListener.AddListener(f);
            this.AddClientListener(cql.clientEventListener, new string[] { cql.query }, new string[] { }, null);
        }

        public static IDictionary<byte[], DotNetClientListener > runnningListeners = new Dictionary<byte[], DotNetClientListener>();

        static void addListener(byte[] listenerId, DotNetClientListener tuple)
        {
            runnningListeners.Add(listenerId, tuple);
        }
        public static void stopAndRemoveTask(byte[] listenerId)
        {
            runnningListeners.Remove(listenerId);
        }



        public void RemoveClientListener(ClientListener<K, V> cl)
        {
            stopAndRemoveTask(cl.listenerId);
            VectorByte vb = new VectorByte(cl.listenerId);
            cache.removeClientListener(vb);
        }

        public void RemoveContinuousQueryListener<CQK, CQV>(Event.ContinuousQueryListener<CQK, CQV> cql)

        {
            stopAndRemoveTask(cql.clientEventListener.listenerId);
            VectorByte vc = new VectorByte(cql.clientEventListener.listenerId);
            cache.removeClientListener(vc);
        }

        private ByteArray wrap(Object input)
        {
            byte[] barray = marshaller.ObjectToByteBuffer(input);
            return new ByteArray(barray, barray.Length);
        }

        private Object unwrap(ByteArray input)
        {
            if (input == null)
            {
                return null;
            }
            byte[] barray = new byte[input.getSize()];
            input.copyBytesTo(barray);
            return marshaller.ObjectFromByteBuffer(barray);
        }

        private Object unwrap(VectorChar input)
        {
            if (input == null)
            {
                return null;
            }
            byte[] barray = new byte[input.Count];
            int i = 0;
            foreach (char c in input)
            {
                barray[i++]=(byte)c;
            }
            return marshaller.ObjectFromByteBuffer(barray);
        }

        private ByteArrayMapInput wrap(IDictionary<K, V> map)
        {
            if (map == null)
            {
                return null;
            }
            ByteArrayMapInput result = new ByteArrayMapInput();
            foreach (KeyValuePair<K, V> current in map)
            {
                result.Add(wrap(current.Key), wrap(current.Value));
            }
            return result;
        }

        private IDictionary<K, V> unwrap(ByteArrayMap map)
        {
            if (map == null)
            {
                return null;
            }
            Dictionary<K, V> result = new Dictionary<K, V>();
            foreach (KeyValuePair<ByteArray, ByteArray> current in map)
            {
                result.Add((K)unwrap(current.Key), (V)unwrap(current.Value));
            }
            return result;
        }

        private IDictionary<String, String> unwrap(StringMap map)
        {
            if (map == null)
            {
                return null;
            }
            Dictionary<String, String> result = new Dictionary<String, String>();
            foreach (KeyValuePair<String, String> current in map)
            {
                result.Add(current.Key, current.Value);
            }
            return result;
        }

        private Infinispan.HotRod.SWIGGen.TimeUnit wrap(TimeUnit timeUnit)
        {
            switch (timeUnit)
            {
                case TimeUnit.NANOSECONDS:
                    return Infinispan.HotRod.SWIGGen.TimeUnit.NANOSECONDS;
                case TimeUnit.MICROSECONDS:
                    return Infinispan.HotRod.SWIGGen.TimeUnit.MICROSECONDS;
                case TimeUnit.MILLISECONDS:
                    return Infinispan.HotRod.SWIGGen.TimeUnit.MILLISECONDS;
                case TimeUnit.SECONDS:
                    return Infinispan.HotRod.SWIGGen.TimeUnit.SECONDS;
                case TimeUnit.MINUTES:
                    return Infinispan.HotRod.SWIGGen.TimeUnit.MINUTES;
                case TimeUnit.HOURS:
                    return Infinispan.HotRod.SWIGGen.TimeUnit.HOURS;
                case TimeUnit.DAYS:
                    return Infinispan.HotRod.SWIGGen.TimeUnit.DAYS;
                default:
                    throw new Infinispan.HotRod.Exceptions.InternalException("no mapping for given time unit");
            }
        }

        private TimeUnit unwrap(Infinispan.HotRod.SWIGGen.TimeUnit timeUnit)
        {
            switch (timeUnit)
            {
                case Infinispan.HotRod.SWIGGen.TimeUnit.NANOSECONDS:
                    return TimeUnit.NANOSECONDS;
                case Infinispan.HotRod.SWIGGen.TimeUnit.MICROSECONDS:
                    return TimeUnit.MICROSECONDS;
                case Infinispan.HotRod.SWIGGen.TimeUnit.MILLISECONDS:
                    return TimeUnit.MILLISECONDS;
                case Infinispan.HotRod.SWIGGen.TimeUnit.SECONDS:
                    return TimeUnit.SECONDS;
                case Infinispan.HotRod.SWIGGen.TimeUnit.MINUTES:
                    return TimeUnit.MINUTES;
                case Infinispan.HotRod.SWIGGen.TimeUnit.HOURS:
                    return TimeUnit.HOURS;
                case Infinispan.HotRod.SWIGGen.TimeUnit.DAYS:
                    return TimeUnit.DAYS;
                default:
                    throw new Infinispan.HotRod.Exceptions.InternalException("no mapping for given time unit");
            }
        }
    }
#pragma warning restore 1591
}
