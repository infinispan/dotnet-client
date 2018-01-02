using Infinispan.HotRod;
using Infinispan.HotRod.Config;
using Infinispan.HotRod.Tests.Util;
using NUnit.Framework;
using System;

namespace Infinispan.HotRod.Tests.StandaloneXml
{
    [TestFixture]
    [Category("standalone_xml")]
    [Category("DefaultTestSuite")]
    public class DefaultCacheForceReturnValueTest
    {
        private IRemoteCache<String, String> cache;

        [OneTimeSetUp]
        public void BeforeClass()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222);
            conf.ConnectionTimeout(90000).SocketTimeout(6000);
            RemoteCacheManager remoteManager = new RemoteCacheManager(conf.Build(), true);
            cache = remoteManager.GetCache<String, String>(true);
        }

        [Test]
        public void PutTest()
        {
            String key = UniqueKey.NextKey();
            ulong initialSize = cache.Size();

            cache.Put(key, "chlorine");
            Assert.AreEqual("chlorine", cache.Put(key, "Berilium"));
            Assert.AreEqual(initialSize + 1, cache.Size());
        }

        [Test]
        public void PutIfAbsentTest()
        {
            String key1 = UniqueKey.NextKey();
            String key2 = UniqueKey.NextKey();

            cache.Put(key1, "carbon0");
            cache.PutIfAbsent(key1, "carbon1");
            cache.PutIfAbsent(key2, "carbon2");
            Assert.AreEqual("carbon0",cache.Get(key1));
            Assert.AreEqual("carbon2", cache.Get(key2));
        }

        [Test]
        public void ReplaceTest()
        {
            String key = UniqueKey.NextKey();

            cache.Put(key, "bromine");
            Assert.AreEqual("bromine", cache.Get(key));
            Assert.AreEqual("bromine",cache.Replace(key, "neon"));
            Assert.AreEqual("neon", cache.Get(key));
        }

        [Test]
        public void ForceReturnValuesTest()
        {
            String key1 = UniqueKey.NextKey();
            String key2 = UniqueKey.NextKey();

            Assert.IsNull(cache.Put(key1, "v1"));
            Assert.AreEqual("v1", cache.Put(key1, "v2"));

            Assert.AreEqual("v2", cache.Remove(key1));

            Assert.IsNull(cache.PutIfAbsent(key2, "v3"));
            Assert.AreEqual("v3", cache.PutIfAbsent(key2, "v4"));
            
            Assert.AreEqual("v3", cache.Replace(key2, "v5"));
            Assert.AreEqual("v5", cache.Get(key2));
        }

    }
}
