using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Util;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotnetClient;

namespace InfinispanDotnetClientSample
{
    class Program
    {
        /// <summary>
        /// This is the Sample Application that uses the Infinispan .NET client library.
        /// </summary>
        /// <param name="args">string</param>
        static void Main(string[] args)
        {
            //Create new Configuration, overriding the setting in the App.config file.
            ClientConfig conf= new ClientConfig("127.0.0.1",11222,"default",0,false);
            //Here we are using a custom Serializer
            Serializer s= new StringSerializer();
            //Create a new RemoteCacheManager
            RemoteCacheManager manager = new RemoteCacheManager(conf, s);
            //Get hold of a cache from the remote cache manager
            RemoteCache cache = manager.getCache();
            
            //First Check Whether the cache exists
            Console.WriteLine("Ping Result : "+cache.ping());
            //Put a new value "germanium" with key "key 1" into cache
            cache.put<String, String>("key 1", "germanium", 0, 0);
            //Get the value of entry with key "key 1"
            Console.WriteLine("key 1 value : "+cache.get<String>("key 1"));
            //Put if absent is used to add entries if they are not existing in the cache
            cache.putIfAbsent<String, String>("key 1", "trinitrotoluene", 0, 0);
            cache.putIfAbsent<String, String>("key 2", "formaldehyde", 0, 0);
            Console.WriteLine("key 1 value after PutIfAbsent: " + cache.get<String>("key 1"));
            Console.WriteLine("Key 2 value after PutIfAbsent: " + cache.get<String>("key 2"));
            //Replace an existing value with a new one.
            cache.replace<String, String>("key 1", "fluoride",0,0);
            Console.WriteLine("key 1 value after replace: " + cache.get<String>("key 1"));
            //Check whether a particular key exists
            Console.WriteLine("key 1 is exist ?: " + cache.containsKey("key 1"));
            //Remove a particular entry from the cache
            cache.remove<String>("key 1");
            Console.WriteLine("key 1 is exist after remove?: " + cache.containsKey("key 1"));

            Console.ReadLine();
        }
    }
}
