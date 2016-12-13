using Infinispan.HotRod.Config;
using NUnit.Framework;
using System;
using Infinispan.HotRod.TestSuites;

namespace Infinispan.HotRod.Tests
{
    public class XSiteFailoverTest
    {
        private RemoteCacheManager manager1;
        private IRemoteCache<String, String> cache1;
        private IRemoteCache<String, String> cache2;

        [TestFixtureSetUp]
        public void BeforeClass()
        {
            ConfigurationBuilder conf1 = new ConfigurationBuilder();
            conf1.AddServer().Host("127.0.0.1").Port(11222);
            conf1.AddCluster("nyc").AddClusterNode("127.0.0.1", 11322);
            manager1 = new RemoteCacheManager(conf1.Build(), true);
            cache1 = manager1.GetCache<String, String>();

            ConfigurationBuilder conf2 = new ConfigurationBuilder();
            conf2.AddServer().Host("127.0.0.1").Port(11322);
            conf2.AddCluster("lon").AddClusterNode("127.0.0.1", 11222);
            RemoteCacheManager remoteManager = new RemoteCacheManager(conf2.Build(), true);
            cache2 = remoteManager.GetCache<String, String>();
        }

        [Test]
        public void FailoverTest()
        {
            Assert.IsNull(cache1.Put("k1", "v1"));
            Assert.AreEqual("v1", cache1.Get("k1"), "Expected v1 from cache1");
            Assert.AreEqual("v1", cache2.Get("k1"), "Expected v1 from cache2");
            XSiteTestSuite.server1.ShutDownHotrodServer();
            //client1 should failover
            Assert.AreEqual("v1", cache1.Get("k1"), "Expected v1 from cache1 after failover");
            Assert.AreEqual("v1", cache2.Get("k1"), "Expected v1 from cache2 after failover");
            XSiteTestSuite.server1.StartHotRodServer();
            manager1.SwitchToDefaultCluster();
            //client1 should get null as state transfer is not enabled
            Assert.IsNull(cache1.Get("k1"));
            Assert.IsNull(cache1.Put("k2", "v2"));
            Assert.AreEqual("v2", cache1.Get("k2"));
            //double check client2
            Assert.AreEqual("v1", cache2.Get("k1"), "Expected v1 from cache2 after starting LON back again");
        }
    }
}