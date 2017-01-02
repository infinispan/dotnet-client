using Infinispan.HotRod.Config;
using System.Collections.Generic;
using Infinispan.HotRod.Tests.Util;
using NUnit.Framework;

namespace Infinispan.HotRod.Tests
{
    class NearCacheTest
    {
        RemoteCacheManager remoteManager;
        const string ERRORS_KEY_SUFFIX = ".errors";
        const string PROTOBUF_SCRIPT_CACHE_NAME = "___script_cache";
        IMarshaller marshaller;

        [TestFixtureSetUp]
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


        [Test]
        public void PopulateNearCacheTest()
        {
            var cache = remoteManager.GetCache<string, string>();
            cache.Clear();
            var stats0 = cache.Stats();
            cache.Get("key1");
            cache.Put("key1", "value1");
            // key1 is near now
            cache.Get("key1");
            var stats1 = cache.Stats();
            // Retrieve stats form the server and do some checks
            // counters don't consider hit and miss on the near cache
            Assert.AreEqual(stats0.GetStatistic("hits"), stats1.GetStatistic("hits"));
            Assert.AreEqual(int.Parse(stats0.GetStatistic("misses"))+1, int.Parse(stats1.GetStatistic("misses")));
            // now fill the near cache
            for (int i=0; i<=10; i++)
            {
                cache.Put("key" + (i+2), "value" + (i+2));
            }
            // key1 is now far
            cache.Get("key1");
            // key1 push key2 out from near cache
            // key2 is now far
            cache.Get("key2");
            // key4 is near
            cache.Get("key4");
            var stats2 = cache.Stats();
            Assert.AreEqual(stats1.GetStatistic("misses"), stats2.GetStatistic("misses"));
            Assert.AreEqual(int.Parse(stats1.GetStatistic("hits")) + 3, int.Parse(stats2.GetStatistic("hits")));
        }
    }
}