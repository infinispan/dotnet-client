﻿using Infinispan.HotRod.Config;
using NUnit.Framework;

namespace Infinispan.HotRod.Tests.StandaloneXml
{
    [TestFixture]
    [Category("standalone_xml")]
    [Category("DefaultTestSuite")]
    public class NearCacheEvictionTest
    {
        RemoteCacheManager remoteManager;
        IMarshaller marshaller;

        [OneTimeSetUp]
        public void BeforeClass()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222)
            .ConnectionTimeout(90000).SocketTimeout(6000)
            .NearCache().Mode(NearCacheMode.INVALIDATED).MaxEntries(10);
            marshaller = new JBasicMarshaller();
            conf.Marshaller(marshaller);
            remoteManager = new RemoteCacheManager(conf.Build(), true);
        }

        [OneTimeTearDown]
        public void AfterClass()
        {
            remoteManager.Stop();
        }

        void checkIsNearWithRetry(IRemoteCache<string,string> cache, string key)
        {

            ServerStatistics stats0 = cache.Stats();
            ServerStatistics stats1 = null;
            for (var i = 0; i < 10; i++)
            {
                cache.Get(key);
                stats1 = cache.Stats();
                if (stats0.GetIntStatistic("nearHits") + 1 == stats1.GetIntStatistic("nearHits"))
                {
                    break;
                }
                System.Threading.Thread.Sleep(200);
            }
            Assert.AreEqual(stats0.GetIntStatistic("nearHits") + 1, stats1.GetIntStatistic("nearHits"));
        }

        [Test]
        public void PutGetTest()
        {
            var cache = remoteManager.GetCache<string, string>();
            cache.Clear();
            var stats0 = cache.Stats();
            cache.Get("key1");
            cache.Put("key1", "value1");
            cache.Get("key1");
            // Check and ensure key1 is near
            checkIsNearWithRetry(cache, "key1");
            var stats1 = cache.Stats();
            // Check also the misses counter
            Assert.AreEqual(stats0.GetIntStatistic("misses")+1, stats1.GetIntStatistic("misses"));
            // now fill the near cache
            for (int i=0; i<10; i++)
            {
                cache.Put("key" + (i+2), "value" + (i+2));
                // populate the near cache
                checkIsNearWithRetry(cache, "key" + (i + 2));
            }
            var stats2 = cache.Stats();
            // key1 is now far
            cache.Get("key1");
            // key1 push key2 out from near cache
            // key2 is now far
            cache.Get("key2");
            // key4 is near
            cache.Get("key3");
            var stats3 = cache.Stats();
            Assert.AreEqual(stats2.GetIntStatistic("misses"), stats3.GetIntStatistic("misses"));
            Assert.AreEqual(stats2.GetIntStatistic("hits") + 3, stats3.GetIntStatistic("hits"));
            cache.Clear();
        }
    }
}
