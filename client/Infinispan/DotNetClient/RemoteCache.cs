using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Operations;

namespace Infinispan.DotNetClient
{
    public interface RemoteCache<K,V>
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

        void putIfAbsent(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis);

        void putIfAbsent(K key, V val);

        V replace(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis);

        V replace(K key, V val);

        bool containsKey(K key);
        
        V get(K key);

        Dictionary<K,V> getBulk(int size);

        Dictionary<K, V> getBulk();

        V remove(K key);
        
        void clear();

        PingOperation.PingResult ping();

        VersionedValue getVersioned(K key);

        VersionedOperationResponse removeIfUnmodified(K key, long version);

        VersionedOperationResponse replaceWithVersion(K key, V val, long version, int lifespaninMillis, int maxIdleTimeinMillis);

        VersionedOperationResponse replaceIfUnmodified(K key, V val, long version);
    }
}
