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

        V putIfAbsent<V, K>(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis);

        V replace<V, K>(K key, V val, int lifespaninMillis, int maxIdleTimeinMillis);

        bool containsKey(Object key);
        
        V get<V>(Object key);

        Dictionary<K, V> getBulk<K, V>(int size);

        Dictionary<K, V> getBulk<K, V>();

        V remove<V>(Object key);
        
        void clear();

        PingOperation.PingResult ping();
    }
}
