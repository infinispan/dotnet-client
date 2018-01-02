using Infinispan.HotRod.Config;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Infinispan.HotRod.Tests.StandaloneXml
{
    [TestFixture]
    [Category("standalone_xml")]
    [Category("DefaultTestSuite")]
    public class NearCacheStaleReadsTest
    {
        RemoteCacheManager remoteManager;
        IMarshaller marshaller;

        [OneTimeSetUp]
        public void BeforeClass()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222)
            .ConnectionTimeout(90000).SocketTimeout(6000)
            .NearCache().Mode(NearCacheMode.INVALIDATED).MaxEntries(-1);
            marshaller = new JBasicMarshaller();
            conf.Marshaller(marshaller);
            remoteManager = new RemoteCacheManager(conf.Build(), true);
        }
        [TestFixtureTearDown]
        public void AfterClass()
        {
          remoteManager.Stop();
        }

        [Test]
        public void AvoidStaleReadAfterPutRemoveTest()
        {
            Repeat((i, cache) =>
            {
                String value = "v" + i;
                cache.Put("k", value);
                Assert.AreEqual(value, cache.Get("k"));
                cache.Remove("k");
                Assert.IsNull(cache.Get("k"));
            });
        }

        [Test]
        public void AvoidStaleReadAfterPutAllTest()
        {
            Repeat((i, cache) =>
            {
                IDictionary<String, String> map = new Dictionary<String, String>();
                String value = "v" + i;
                map.Add("k", value);
                cache.PutAll(map);
                Assert.AreEqual(value, cache.Get("k"));
            });
        }

        [Test]
        public void AvoidStaleReadAfterReplaceTest()
        {
            Repeat((i, cache) =>
            {
                String value = "v" + i;
                cache.Replace("k", value);
                IVersionedValue<String> versioned = cache.GetVersioned("k");
                Assert.AreEqual(value, versioned.GetValue());
            });
        }

        [Test]
        public void AvoidStaleReadAfterReplaceWithVersionTest()
        {
            Repeat((i, cache) =>
            {
                String value = "v" + i;
                IVersionedValue<String> versioned = cache.GetVersioned("k");
                Assert.IsTrue(cache.ReplaceWithVersion("k", value, (ulong) versioned.GetVersion()), "Not replaced!");
                Assert.AreEqual(value, cache.Get("k"));
            });
        }

        [Test]
        public void AvoidStaleReadAfterPutAsyncRemoveWithVersionTest()
        {
            Repeat((i, cache) =>
            {
                String value = "v" + i;
                cache.PutAsync("k", value).Wait();
                IVersionedValue<String> versioned = cache.GetVersioned("k");
                Assert.AreEqual(value, versioned.GetValue());
                Assert.IsTrue(cache.RemoveWithVersion("k", (ulong) versioned.GetVersion()), "Not removed!");
                Assert.IsNull(cache.Get("k"));
            });
        }

        private void Repeat(Action<int, IRemoteCache<String, String>> f)
        {
            var cache = remoteManager.GetCache<string, string>();
            cache.Clear();
            cache.PutIfAbsent("k", "v0");
            for (int i = 1; i != 1000; i++)
            {
                f.Invoke(i, cache);
            }
        }
    }
}
