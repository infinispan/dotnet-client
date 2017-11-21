using System;
using Infinispan.HotRod.Config;

namespace Infinispan.HotRod.SimpleSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            (var key, var value) = ("1", "hello");
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddServer().Host("127.0.0.1").Port(11222);
            configurationBuilder.ConnectionTimeout(90000).SocketTimeout(6000);
            var remoteManager = new RemoteCacheManager(configurationBuilder.Build());
            var cache = remoteManager.GetCache<string, string>();
            cache.PutIfAbsent(key, value);
            var outOfCache = cache.Get(key);
            Console.WriteLine($"{key}: {outOfCache}");
        }
    }
}
