using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinispan.HotRod.Wrappers
{
    public class RemoteCache
    {
        private Infinispan.HotRod.IRemoteCache<byte[], byte[]> cache;

        internal RemoteCache(Infinispan.HotRod.IRemoteCache<byte[], byte[]> cache)
        {
            this.cache = cache;
        }

        public string GetName()
        {
            return cache.GetName();
        }

        public string GetVersion()
        {
            return cache.GetVersion();
        }

        public string GetProtocolVersion()
        {
            return cache.GetProtocolVersion();
        }

        public ulong Size()
        {
            return cache.Size();
        }

        public bool IsEmpty()
        {
            return cache.IsEmpty();
        }

        public String[][] Stats()
        {
            IDictionary<String, String> statsMap = cache.Stats().GetStatsMap();
            String[][] result = new String[statsMap.Keys.Count()][];
            
            int count = 0;
            foreach (KeyValuePair<String, String> kvp in statsMap) {
                result[count] = new String[2];
                result[count][0] = kvp.Key;
                result[count][1] = kvp.Value;
                count++;
            }
            return result;
        }

        public byte[] Put(byte[] key, byte[] val, ulong lifespan, TimeUnit lifespanUnit)
        {
            return cache.Put(key, val, lifespan, lifespanUnit);
        }

        public byte[] Put(byte[] key, byte[] val, ulong lifespan, TimeUnit lifespanUnit, ulong maxIdleTime, TimeUnit maxIdleUnit)
        {
            return cache.Put(key, val, lifespan, lifespanUnit, maxIdleTime, maxIdleUnit);
        }

        public byte[] Put(byte[] key, byte[] val)
        {
            return cache.Put(key, val);
        }

        public byte[] PutIfAbsent(byte[] key, byte[] val, ulong lifespan, TimeUnit lifespanUnit, ulong maxIdleTime, TimeUnit maxIdleUnit)
        {
            return cache.PutIfAbsent(key, val, lifespan, lifespanUnit, maxIdleTime, maxIdleUnit);
        }

        public byte[] PutIfAbsent(byte[] key, byte[] val, ulong lifespan, TimeUnit lifespanUnit)
        {
            return cache.PutIfAbsent(key, val, lifespan, lifespanUnit);
        }

        public byte[] PutIfAbsent(byte[] key, byte[] val)
        {
            return cache.PutIfAbsent(key, val);
        }

        // public void PutAll(IDictionary<K, V> map, ulong lifespan, TimeUnit lifespanUnit, ulong maxIdleTime, TimeUnit maxIdleUnit)
        // {
        // }

        // public void PutAll(IDictionary<K, V> map, ulong lifespan, TimeUnit lifespanUnit)
        // {
        // }

        // public void PutAll(IDictionary<K, V> map)
        // {
        // }

        public byte[] Replace(byte[] key, byte[] val, ulong lifespan, TimeUnit lifespanUnit, ulong maxIdleTime, TimeUnit maxIdleUnit)
        {
            return cache.Replace(key, val, lifespan, lifespanUnit, maxIdleTime, maxIdleUnit);
        }

        public byte[] Replace(byte[] key, byte[] val, ulong lifespan, TimeUnit lifespanUnit)
        {
            return cache.Replace(key, val, lifespan, lifespanUnit);
        }

        public byte[] Replace(byte[] key, byte[] val)
        {
            return cache.Replace(key, val);
        }

        public bool ContainsKey(byte[] key)
        {
            return cache.ContainsKey(key);
        }

        public bool ContainsValue(byte[] val)
        {
            return cache.ContainsValue(val);
        }

        public byte[] Get(byte[] key)
        {
            return cache.Get(key);
        }

        public byte[][][] GetBulk(int size)
        {
            return toByteArray(cache.GetBulk(size));

        }

        public byte[][][] GetBulk()
        {
            return toByteArray(cache.GetBulk());
        }

        public byte[] Remove(byte[] key)
        {
            return cache.Remove(key);
        }

        public void Clear()
        {
            cache.Clear();
        }

        public VersionedValue GetVersioned(byte[] key)
        {
            IVersionedValue<byte[]> result = cache.GetVersioned(key);
            if (result == null) {
                return null;
            }
            return new VersionedValue(result);
        }

        public MetadataValue GetWithMetadata(byte[] key)
        {
            IMetadataValue<byte[]> result = cache.GetWithMetadata(key);
            if (result == null) {
                return null;
            }
            return new MetadataValue(result);
        }

        public bool RemoveWithVersion(byte[] key, ulong version)
        {
            return cache.RemoveWithVersion(key, version);
        }

        public bool ReplaceWithVersion(byte[] key, byte[] val, ulong version, ulong lifespan, ulong maxIdleTime)
        {
            return cache.ReplaceWithVersion(key, val, version, lifespan, maxIdleTime);
        }

        public bool ReplaceWithVersion(byte[] key, byte[] val, ulong version, ulong lifespan)
        {
            return cache.ReplaceWithVersion(key, val, version, lifespan);
        }

        public bool ReplaceWithVersion(byte[] key, byte[] val, ulong version)
        {
            return cache.ReplaceWithVersion(key, val, version);
        }

        // public ISet<KeyValuePair<K, V>> EntrySet()
        // {
        // }

        public byte[][] KeySet()
        {
            return toByteArray(cache.KeySet());
        }

        // public IList<V> Values()
        // {
        // }

        public RemoteCache WithFlags(Flags flags)
        {
            return new RemoteCache(cache.WithFlags(flags));
        }

        private byte[][] toByteArray(ISet<byte[]> data)
        {
            byte[][] result = null;
            if (data != null) {
                result = new byte[data.Count()][];
                int count = 0;
                foreach (byte[] item in data) {
                    result[count++] = item;
                }
            }
            return result;
        }

        private byte[][][] toByteArray(IDictionary<byte[], byte[]> data)
        {
            byte[][][] result = null;
            if (data != null) {
                result = new byte[data.Count()][][];
                int count = 0;
                foreach (KeyValuePair<byte[], byte[]> kvp in data) {
                    result[count] = new byte[2][];
                    result[count][0] = kvp.Key;
                    result[count][1] = kvp.Value;
                    count++;
                }
            }
            return result;
        }
    }
}
