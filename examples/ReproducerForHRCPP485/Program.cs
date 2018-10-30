using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinispan.HotRod.Config;
using Infinispan.HotRod;
using System.Threading;

namespace ConsoleApplication5
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

            string key = "prefix_for_key_";
            string val = "prefix_for_value_";
            var ts = new Thread[50];
            for (int i = 0; i < 10; i++)
            {
                ts[i] = new Thread(() => NewMethod(cache, "th" + i + key, "th" + i + val));
                ts[i].Start();
            }
            ts[0].Join();
            rcm.Stop();
        }

        private static void NewMethod(IRemoteCache<string, string> cache, string key, string val)
        {
            var count = 0;
            while (true)
            {
                ++count;
                if (count % 1000 == 0)
                    System.Console.Write("." + count + " " + System.Threading.Thread.CurrentThread);
                cache.Put(key + count, val + count);
                if (count % 1000 == 0)
                    System.Console.WriteLine("+" + count);
                System.Threading.Thread.Sleep(10);
                cache.Get(key);
                System.Threading.Thread.Sleep(10);
            }
        }
    }
}
