using Infinispan.HotRod.Config;
using NUnit.Framework;
using System;
using Infinispan.HotRod.TestSuites;
using Infinispan.HotRod.Transport;
using System.Collections.Generic;
using Infinispan.HotRod.Tests;
using Infinispan.HotRod.Tests.Util;

namespace Infinispan.HotRod.Tests
{
    public class BS : Infinispan.HotRod.Transport.FailOverRequestBalancingStrategy
    {
        IList<InetSocketAddress> servers;
        public InetSocketAddress nextServer(IList<InetSocketAddress> failedServer)
        {
            // Choose the first server not in the failedServer list
            foreach(var server in servers)
            {
                if (!failedServer.Contains(server))
                    return server;
            }
            throw new Exception("No transport");
        }

        public void setServers(IList<InetSocketAddress> servers)
        {
            this.servers = servers;
        }
    }
    [TestFixture]
    [Category("clustered_xsite_xml_2")]
public class XSiteFailoverTest : XSiteTestSuite
    {
        RemoteCacheManager remoteManager;
        private RemoteCacheManager manager1;
        private IRemoteCache<String, String> cache1;
        private IRemoteCache<String, String> cache2;
        ConfigurationBuilder conf1;
        ConfigurationBuilder conf2;
        Configuration configu1;
        Configuration configu2;

        [OneTimeSetUp]
        public void BeforeClass()
        {
            BS b;
            FailOverRequestBalancingStrategyProducerDelegate d = delegate ()
            {
                b = new BS();
                return b;
            };
            conf1 = new ConfigurationBuilder();
            conf1.AddServer().Host("127.0.0.1").Port(11222);
            conf1.AddCluster("nyc").AddClusterNode("127.0.0.1", 11322);
            conf1.BalancingStrategyProducer(d);
            configu1 = conf1.Build();
            manager1 = new RemoteCacheManager(configu1, true);
            cache1 = manager1.GetCache<String, String>();

            conf2 = new ConfigurationBuilder();
            conf2.AddServer().Host("127.0.0.1").Port(11322);
            conf2.AddCluster("lon").AddClusterNode("127.0.0.1", 11222);
            configu2 = conf2.Build();
            remoteManager = new RemoteCacheManager(configu2, true);
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
