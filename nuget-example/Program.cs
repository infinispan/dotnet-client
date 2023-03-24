using System;
using Infinispan.HotRod.Config;
using Infinispan.HotRod;

namespace ConsoleApplication
{
    class Loader
    {

    }
    class Program
    {
        static void Main(string[] args)
        {
            var confBuilder = new ConfigurationBuilder();
            confBuilder.AddServer().Host("127.0.0.1").Port(11222);
            var rcm = new RemoteCacheManager(confBuilder.Build());
            rcm.Start();
            var cache = rcm.GetCache<string, string>();
            cache.Put("myKey", "myValue");
            System.Console.WriteLine("myKey => " + cache.Get("myKey"));
 
            rcm.Stop();
        }
    }
}
