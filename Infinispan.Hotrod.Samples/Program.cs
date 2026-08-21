using System;
using System.Threading.Tasks;
using Infinispan.Hotrod;
namespace Infinispan.Hotrod.Samples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var ispnCluster = new InfinispanClient();

            // Configuration section
            ispnCluster.User = "admin";
            ispnCluster.Password = "admin";
            ispnCluster.AuthMech = "PLAIN";
            ispnCluster.Version = ProtocolVersion.Version31;
            ispnCluster.ClientIntelligence = ClientIntelligence.HashDistributionAware;
            ispnCluster.ForceReturnValue = false;

            var host = ispnCluster.AddHost("127.0.0.1", 11222);
            await Test(ispnCluster);
        }
        static async Task Test(InfinispanClient ispnCluster)
        {
            var km = new StringMarshaller();
            var vm = new StringMarshaller();
            var cache = ispnCluster.NewCache(km, vm, "distributed");
            cache.ForceReturnValue = true;
            string result = await cache.Put("key1", "value1");
            Console.WriteLine("Result is: " + result);
            string getResult = await cache.Get("key1");
            Console.WriteLine("Get Result is: " + getResult);
        }
    }
}
