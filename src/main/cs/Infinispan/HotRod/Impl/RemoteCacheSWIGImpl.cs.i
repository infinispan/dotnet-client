using System;
using System.Diagnostics;
using System.Collections.Generic;
using Infinispan.HotRod.SWIG${ARCH};
using Infinispan.HotRod.Exceptions;
using Org.Infinispan.Query.Remote.Client;
using System.IO;
using Google.Protobuf;

namespace Infinispan.HotRod.Impl
{
#pragma warning disable 1591
    internal class RemoteCacheSWIG${ARCH}Impl<K, V> : IRemoteCache<K, V>
    {
        private RemoteByteArrayCache cache;
        private IMarshaller marshaller;

        public RemoteCacheSWIG${ARCH}Impl(Infinispan.HotRod.SWIG.RemoteByteArrayCache cache, IMarshaller marshaller)
        {
            this.cache = (RemoteByteArrayCache) cache;
            this.marshaller = marshaller;
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
            if (swigResult == null) {
                return null;
            }
            return new ServerStatisticsImpl(unwrap(swigResult));
        }

        public V Put(K key, V val, ulong lifespan, TimeUnit lifespanUnit)
        {
            return (V) unwrap(cache.put(wrap(key), wrap(val), lifespan, wrap(lifespanUnit)));
        }

        public V Put(K key, V val, ulong lifespan, TimeUnit lifespanUnit, ulong maxIdleTime, TimeUnit maxIdleUnit)
        {
            return (V) unwrap(cache.put(wrap(key), wrap(val), lifespan, wrap(lifespanUnit), maxIdleTime, wrap(maxIdleUnit)));
        }

        public V Put(K key, V val)
        {
            return (V) unwrap(cache.put(wrap(key), wrap(val)));
        }

        public V PutIfAbsent(K key, V val, ulong lifespan, TimeUnit lifespanUnit, ulong maxIdleTime, TimeUnit maxIdleUnit)
        {
            return (V) unwrap(cache.putIfAbsent(wrap(key), wrap(val), lifespan, wrap(lifespanUnit), maxIdleTime, wrap(maxIdleUnit)));
        }

        public V PutIfAbsent(K key, V val, ulong lifespan, TimeUnit lifespanUnit)
        {
            return (V) unwrap(cache.putIfAbsent(wrap(key), wrap(val), lifespan, wrap(lifespanUnit)));
        }

        public V PutIfAbsent(K key, V val)
        {
            return (V) unwrap(cache.putIfAbsent(wrap(key), wrap(val)));
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
            return (V) unwrap(cache.replace(wrap(key), wrap(val), lifespan, wrap(lifespanUnit), maxIdleTime, wrap(maxIdleUnit)));
        }

        public V Replace(K key, V val, ulong lifespan, TimeUnit lifespanUnit)
        {
            return (V) unwrap(cache.replace(wrap(key), wrap(val), lifespan, wrap(lifespanUnit)));
        }

        public V Replace(K key, V val)
        {
            return (V) unwrap(cache.replace(wrap(key), wrap(val)));
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
            return (V) unwrap(cache.get(wrap(key)));
        }

        public IDictionary<K, V> GetBulk(int size)
        {
            return unwrap(cache.getBulk(size));
        }

        public IDictionary<K, V> GetBulk()
        {
            return unwrap(cache.getBulk());
        }

        public V Remove(K key)
        {
            return (V) unwrap(cache.remove(wrap(key)));
        }

        public void Clear()
        {
            cache.clear();
        }

        public IVersionedValue<V> GetVersioned(K key)
        {
            ValueVersionPair pair = cache.getWithVersion(wrap(key));
            if (pair == null || pair.first == null) {
                return null;
            }
            return new VersionedValueImpl<V>((V) unwrap(pair.first),
                                             pair.second.version);
        }

        public IMetadataValue<V> GetWithMetadata(K key)
        {
            ValueMetadataPair pair = cache.getWithMetadata(wrap(key));
            if (pair == null || pair.first == null) {
                return null;
            }
            return new MetadataValueImpl<V>((V) unwrap(pair.first),
                                            pair.second.version,
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
                    result.Add((K) unwrap(item));
                }
            }
            
            return result;
        }

        public IList<V> Values()
        {
            throw new Infinispan.HotRod.Exceptions.UnsupportedOperationException("Unsupported operation.");
        }

        public IRemoteCache<K, V> WithFlags(Flags flags)
        {
            cache.withFlags((Infinispan.HotRod.SWIG${ARCH}.Flag) flags);
            return this;
        }


        public QueryResponse Query(QueryRequest qr)
        {
            uint size = (uint) qr.CalculateSize();
            byte[] bytes = new byte[size];
            CodedOutputStream cos = new CodedOutputStream(bytes);
            qr.WriteTo(cos);
            cos.Flush();
            string s2 = System.Text.Encoding.ASCII.GetString(bytes);
            VectorByte vb = new VectorByte(bytes);
            VectorByte resp= cache.query(vb, size);
            byte[] respBytes = new byte[resp.Count];
            resp.CopyTo(respBytes);
            QueryResponse queryResp = new QueryResponse();
            return QueryResponse.Parser.ParseFrom(respBytes);
        }
        
        public byte[] Execute(string scriptName, IDictionary<string, string> dict)
        {
            StringMap sm = new StringMap();
            foreach (KeyValuePair<string, string> p in dict)
            {
                sm.Add(p.Key, p.Value);
            }
            VectorByte vb = cache.execute(scriptName, sm);
            byte[] ret = new byte[vb.Count];
            vb.CopyTo(ret);
            return ret;
        }

        private ByteArray wrap(Object input)
        {
            byte[] barray = marshaller.ObjectToByteBuffer(input);
            return new ByteArray(barray, barray.Length);
        }

        private Object unwrap(ByteArray input)
        {
            if (input == null) {
                return null;
            }
            byte[] barray = new byte[input.getSize()];
            input.copyBytesTo(barray);
            return marshaller.ObjectFromByteBuffer(barray);
        }

        private ByteArrayMapInput wrap(IDictionary<K, V> map)
        {
            if (map == null) {
                return null;
            }
            ByteArrayMapInput result = new ByteArrayMapInput();
            foreach (KeyValuePair<K, V> current in map) {
                result.Add(wrap(current.Key), wrap(current.Value));
            }
            return result;
        }

        private IDictionary<K, V> unwrap(ByteArrayMap map)
        {
            if (map == null) {
                return null;
            }
            Dictionary<K, V> result = new Dictionary<K, V>();
            foreach (KeyValuePair<ByteArray, ByteArray> current in map) {
                result.Add((K) unwrap(current.Key), (V) unwrap(current.Value));
            }
            return result;
        }

        private IDictionary<String, String> unwrap(StringMap map)
        {
            if (map == null) {
                return null;
            }
            Dictionary<String, String> result = new Dictionary<String, String>();
            foreach (KeyValuePair<String, String> current in map) {
                result.Add(current.Key, current.Value);
            }
            return result;
        }

        private Infinispan.HotRod.SWIG${ARCH}.TimeUnit wrap(TimeUnit timeUnit)
        {
            switch (timeUnit) {
            case TimeUnit.NANOSECONDS:
                return Infinispan.HotRod.SWIG${ARCH}.TimeUnit.NANOSECONDS;
            case TimeUnit.MICROSECONDS:
                return Infinispan.HotRod.SWIG${ARCH}.TimeUnit.MICROSECONDS;
            case TimeUnit.MILLISECONDS:
                return Infinispan.HotRod.SWIG${ARCH}.TimeUnit.MILLISECONDS;
            case TimeUnit.SECONDS:
                return Infinispan.HotRod.SWIG${ARCH}.TimeUnit.SECONDS;
            case TimeUnit.MINUTES:
                return Infinispan.HotRod.SWIG${ARCH}.TimeUnit.MINUTES;
            case TimeUnit.HOURS:
                return Infinispan.HotRod.SWIG${ARCH}.TimeUnit.HOURS;
            case TimeUnit.DAYS:
                return Infinispan.HotRod.SWIG${ARCH}.TimeUnit.DAYS;
            default:
                throw new Infinispan.HotRod.Exceptions.InternalException("no mapping for given time unit");
            }
        }

        private TimeUnit unwrap(Infinispan.HotRod.SWIG${ARCH}.TimeUnit timeUnit)
        {
            switch (timeUnit) {
            case Infinispan.HotRod.SWIG${ARCH}.TimeUnit.NANOSECONDS:
                return TimeUnit.NANOSECONDS;
            case Infinispan.HotRod.SWIG${ARCH}.TimeUnit.MICROSECONDS:
                return TimeUnit.MICROSECONDS;
            case Infinispan.HotRod.SWIG${ARCH}.TimeUnit.MILLISECONDS:
                return TimeUnit.MILLISECONDS;
            case Infinispan.HotRod.SWIG${ARCH}.TimeUnit.SECONDS:
                return TimeUnit.SECONDS;
            case Infinispan.HotRod.SWIG${ARCH}.TimeUnit.MINUTES:
                return TimeUnit.MINUTES;
            case Infinispan.HotRod.SWIG${ARCH}.TimeUnit.HOURS:
                return TimeUnit.HOURS;
            case Infinispan.HotRod.SWIG${ARCH}.TimeUnit.DAYS:
                return TimeUnit.DAYS;
            default:
                throw new Infinispan.HotRod.Exceptions.InternalException("no mapping for given time unit");
            }
        }
    }
#pragma warning restore 1591
}
