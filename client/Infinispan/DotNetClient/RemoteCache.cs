using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Operations;

namespace Infinispan.DotNetClient
{
    public interface RemoteCache
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

        void put<V, K>(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis);

        void put<V, K>(K key, V val);

        void putIfAbsent<V, K>(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis);

        void putIfAbsent<V, K>(K key, V val);

        V replace<V, K>(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis);

        V replace<V, K>(K key, V val);

        bool containsKey<K>(K key);
        
        V get<V,K>(K key);

        Dictionary<K, V> getBulk<K, V>(int size);

        Dictionary<K, V> getBulk<K, V>();

        V remove<K, V>(K key);
        
        void clear();

        PingOperation.PingResult ping();

        VersionedValue getVersioned<K, V>(K key);

        VersionedOperationResponse removeIfUnmodified<K>(K key, long version);

        VersionedOperationResponse replaceIfUnmodified<K, V>(K key, V val, long version, int lifespaninMillis, int maxIdleTimeinMillis);

        VersionedOperationResponse replaceIfUnmodified<K, V>(K key, V val, long version);
    }
}
