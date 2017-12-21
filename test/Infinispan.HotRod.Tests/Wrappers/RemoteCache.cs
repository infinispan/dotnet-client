using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinispan.HotRod.Wrappers
{
    public class DictionaryOfObjects : Dictionary<object, object>
    {
    }
    public class RemoteCache
    {
        private Infinispan.HotRod.IRemoteCache<object, object> cache;
        internal RemoteCache(Infinispan.HotRod.IRemoteCache<object, object> cache)
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
            foreach (KeyValuePair<String, String> kvp in statsMap)
            {
                result[count] = new String[2];
                result[count][0] = kvp.Key;
                result[count][1] = kvp.Value;
                count++;
            }
            return result;
        }

        public object Put(object key, object val, ulong lifespan, TimeUnit lifespanUnit)
        {
            return cache.Put(key, val, lifespan, lifespanUnit);
        }

        public object Put(object key, object val, ulong lifespan, TimeUnit lifespanUnit, ulong maxIdleTime, TimeUnit maxIdleUnit)
        {
            return cache.Put(key, val, lifespan, lifespanUnit, maxIdleTime, maxIdleUnit);
        }

        public object Put(object key, object val)
        {
            return cache.Put(key, val);
        }

        public object PutIfAbsent(object key, object val, ulong lifespan, TimeUnit lifespanUnit, ulong maxIdleTime, TimeUnit maxIdleUnit)
        {
            return cache.PutIfAbsent(key, val, lifespan, lifespanUnit, maxIdleTime, maxIdleUnit);
        }

        public object PutIfAbsent(object key, object val, ulong lifespan, TimeUnit lifespanUnit)
        {
            return cache.PutIfAbsent(key, val, lifespan, lifespanUnit);
        }

        public object PutIfAbsent(object key, object val)
        {
            return cache.PutIfAbsent(key, val);
        }

        public void PutAll(IDictionary<object, object> map, ulong lifespan, TimeUnit lifespanUnit, ulong maxIdleTime, TimeUnit maxIdleUnit)
        {
            cache.PutAll(map, lifespan, lifespanUnit, maxIdleTime, maxIdleUnit);
        }

        public void PutAll(IDictionary<object, object> map, ulong lifespan, TimeUnit lifespanUnit)
        {
            cache.PutAll(map, lifespan, lifespanUnit);
        }

        public void PutAll(IDictionary<object, object> map)
        {
            cache.PutAll(map);
        }

        public object Replace(object key, object val, ulong lifespan, TimeUnit lifespanUnit, ulong maxIdleTime, TimeUnit maxIdleUnit)
        {
            return cache.Replace(key, val, lifespan, lifespanUnit, maxIdleTime, maxIdleUnit);
        }

        public object Replace(object key, object val, ulong lifespan, TimeUnit lifespanUnit)
        {
            return cache.Replace(key, val, lifespan, lifespanUnit);
        }

        public object Replace(object key, object val)
        {
            return cache.Replace(key, val);
        }

        public bool ContainsKey(object key)
        {
            return cache.ContainsKey(key);
        }

        public bool ContainsValue(object val)
        {
            return cache.ContainsValue(val);
        }

        public object Get(object key)
        {
            return cache.Get(key);
        }

        public object[][] GetBulk(int size)
        {
            return toObjectArray(cache.GetBulk(size));

        }

        public object[][] GetBulk()
        {
            return toObjectArray(cache.GetBulk());
        }

        public object Remove(object key)
        {
            return cache.Remove(key);
        }

        public void Clear()
        {
            cache.Clear();
        }

        public VersionedValue GetVersioned(object key)
        {
            IVersionedValue<object> result = cache.GetVersioned(key);
            if (result == null)
            {
                return null;
            }
            return new VersionedValue(result);
        }

        public MetadataValue GetWithMetadata(object key)
        {
            IMetadataValue<object> result = cache.GetWithMetadata(key);
            if (result == null)
            {
                return null;
            }
            return new MetadataValue(result);
        }

        public bool RemoveWithVersion(object key, ulong version)
        {
            return cache.RemoveWithVersion(key, version);
        }

        public bool ReplaceWithVersion(object key, object val, ulong version, ulong lifespan, ulong maxIdleTime)
        {
            return cache.ReplaceWithVersion(key, val, version, lifespan, maxIdleTime);
        }

        public bool ReplaceWithVersion(object key, object val, ulong version, ulong lifespan)
        {
            return cache.ReplaceWithVersion(key, val, version, lifespan);
        }

        public bool ReplaceWithVersion(object key, object val, ulong version)
        {
            return cache.ReplaceWithVersion(key, val, version);
        }

        // public ISet<KeyValuePair<K, V>> EntrySet()
        // {
        // }

        public object[] KeySet()
        {
            return toObjectArray(cache.KeySet());
        }

        // public IList<V> Values()
        // {
        // }

        public RemoteCache WithFlags(Flags flags)
        {
            return new RemoteCache(cache.WithFlags(flags));
        }

        private object[] toObjectArray(ISet<object> data)
        {
            object[] result = null;
            if (data != null)
            {
                result = new byte[data.Count()][];
                int count = 0;
                foreach (object item in data)
                {
                    result[count++] = item;
                }
            }
            return result;
        }

        public Task<object> PutAsync(object key, object value)
        {
            return cache.PutAsync(key, value);
        }

        public Task PutAllAsync(IDictionary<object, object> map)
        {
            return cache.PutAllAsync(map);
        }

        public Task PutAllAsync(IDictionary<object, object> map, ulong lifespan, TimeUnit lifespanUnit)
        {
            return cache.PutAllAsync(map, lifespan, lifespanUnit);
        }

        public Task PutAllAsync(IDictionary<object, object> map, ulong lifespan, TimeUnit lifespanUnit, ulong maxIdleTime, TimeUnit maxIdleUnit)
        {
            return cache.PutAllAsync(map, lifespan, lifespanUnit, maxIdleTime, maxIdleUnit);
        }

        public Task<object> GetAsync(object key)
        {
            return cache.GetAsync(key);
        }


        public Task<object> ReplaceAsync(object key, object value)
        {
            return cache.ReplaceAsync(key, value);
        }

        public Task<bool> ReplaceWithVersionAsync(object key, object val, ulong version)
        {
            return cache.ReplaceWithVersionAsync(key, val, version);
        }

        public Task<bool> ReplaceWithVersionAsync(object key, object val, ulong version, ulong lifeSpan, ulong maxIdleTime)
        {
            return cache.ReplaceWithVersionAsync(key, val, version, lifeSpan, maxIdleTime);
        }

        public Task<bool> ReplaceWithVersionAsync(object key, object val, ulong version, ulong lifeSpan)
        {
            return cache.ReplaceWithVersionAsync(key, val, version, lifeSpan);
        }

        public Task<bool> RemoveWithVersionAsync(object key, ulong version)
        {
            return cache.RemoveWithVersionAsync(key, version);
        }

        public Task<object> RemoveAsync(object key)
        {
            return cache.RemoveAsync(key);
        }

        public Task<object> PutIfAbsentAsync(object key, object value)
        {
            return cache.PutIfAbsentAsync(key, value);
        }

        public object taskResult(Task t)
        {
            var res = ((Task<object>)t).Result;
            return res;
        }

        public bool taskResultBool(Task t)
        {
            var res = ((Task<bool>)t).Result;
            return res;
        }

        public void taskResultVoid(Task t)
        {
            t.Wait();
        }


        private object[][] toObjectArray(IDictionary<object, object> data)
        {
            object[][] result = null;
            if (data != null)
            {
                result = new object[data.Count()][];
                int count = 0;
                foreach (KeyValuePair<object, object> kvp in data)
                {
                    result[count] = new object[2];
                    result[count][0] = kvp.Key;
                    result[count][1] = kvp.Value;
                    count++;
                }
            }
            return result;
        }
    }
}
