using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Infinispan.HotRod;
using Infinispan.HotRod.Config;


namespace nativeTestSuite
{
    class SSLTest
    {
        public static int testMain(string[] args)
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222).ConnectionTimeout(90000).SocketTimeout(900);
            SslConfigurationBuilder sslConfB = conf.Ssl();
            sslConfB.Enable().ServerCAFile("c:\\tmp\\infinispan-ca.pem");
            if (args.Length > 2)
            {
                sslConfB.ClientCertificateFile(args[2]);
            }

            IMarshaller m = new JBasicMarshaller();
            conf.Marshaller(m);
            RemoteCacheManager remote = new RemoteCacheManager(conf.Build(), true);
            IRemoteCache<String, String> testCache = remote.GetCache<String, String>();
            testCache.Clear();
            string k1="key13";
            string v1="boron";

            testCache.Put(k1, v1);
            string rv=testCache.Get(k1);
            if (!rv.Equals(v1))
            {
                System.Console.WriteLine("get/put fail for "+ k1 + " got " + rv + " expected " + v1);
                return 1;
            }

            return 0;

        }
    }
}
