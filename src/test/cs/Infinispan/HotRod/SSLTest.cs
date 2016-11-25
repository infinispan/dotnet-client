using System;
using NUnit.Framework;
using Infinispan.HotRod.Config;

namespace Infinispan.HotRod.Tests
{
    class SSLTest
    {
        RemoteCacheManager remoteManager;

        [TestFixtureSetUp]
        public void BeforeClass()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222);
            conf.ConnectionTimeout(90000).SocketTimeout(900);
            SslConfigurationBuilder sslConfB = conf.Ssl();
            if (!System.IO.File.Exists("infinispan-ca.pem"))
            {
                Console.WriteLine("File not found: infinispan-ca.pem.");
                Environment.Exit(-1);
            }
            sslConfB.Enable().ServerCAFile("infinispan-ca.pem");
	    // No trusted certificate for now
            //if (!System.IO.File.Exists("client-ca.pem"))
            //{
            //    Console.WriteLine("File not found: client-ca.pem.");
            //    Environment.Exit(-1);
            //}
            //sslConfB.ClientCertificateFile("client-ca.pem");

            conf.Marshaller(new JBasicMarshaller());
            remoteManager = new RemoteCacheManager(conf.Build(), true);
        }

        [Test]
        public void BasicSSLTest()
        {
            IRemoteCache<string, string> testCache = remoteManager.GetCache<string, string>();
            testCache.Clear();
            string k1 = "key13";
            string v1 = "boron";
            testCache.Put(k1, v1);
            Assert.AreEqual(v1, testCache.Get(k1));
        }
    }
}
