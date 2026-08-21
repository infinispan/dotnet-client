using System;
using System.Threading.Tasks;
using Infinispan.Hotrod;
namespace Infinispan.Hotrod.Application
{

    class Program
    {
        static async Task Main(string[] args)
        {
            var myKey = "myKey";
            /// [Create a cluster object]
            InfinispanClient dg = new InfinispanClient();
            // Use a non-authenticated non-encrypted cluster;
            dg.AddHost("127.0.0.1", 11222);
            /// [Create a cluster object]
            /// [Create a cache]
            var cache = dg.NewCache<string, string>(new StringMarshaller(), new StringMarshaller(), "default");
            /// [Create a cache]
            /// [Application code]
            await cache.Put(myKey, "some value");
            string result = await cache.Get(myKey);
            Console.WriteLine("Getting my entry with key {0} from the cache. Result value is: {1}", myKey, result);
            /// [Application code]
        }
    }
}
