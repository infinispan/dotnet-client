using Infinispan.HotRod;
using Infinispan.HotRod.Config;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Infinispan.HotRod.Tests.Util;
using System.Threading.Tasks;
using System.Threading;

namespace Infinispan.HotRod.Tests.StandaloneXml
{
    [TestFixture]
    [Category("standalone_xml")]
    [Category("DefaultTestSuite")]
    public class DefaultCacheTest
    {
        private IRemoteCache<String, String> cache;

        [OneTimeSetUp]
        public void BeforeClass()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222);
            conf.ConnectionTimeout(90000).SocketTimeout(6000);
            RemoteCacheManager remoteManager = new RemoteCacheManager(conf.Build(), true);
            cache = remoteManager.GetCache<String, String>();
        }

        [Test]
        public void NameTest()
        {
            Assert.IsNotNull(cache.GetName());
        }

        [Test]
        public void VersionTest()
        {
            Assert.IsNotNull(cache.GetVersion());
        }

        [Test]
        public void ProtocolVersionTest()
        {
            Assert.IsNotNull(cache.GetProtocolVersion());
        }

        [Test]
        public void GetTest()
        {
            String key = UniqueKey.NextKey();

            Assert.IsNull(cache.Get(key));
            cache.Put(key, "carbon");
            Assert.AreEqual("carbon", cache.Get(key));
        }

        [Test]
        public void GetAllTest()
        {
            String key1 = UniqueKey.NextKey();
            String key2 = UniqueKey.NextKey();
            cache.Clear();
            Assert.IsNull(cache.Get(key1));
            Assert.IsNull(cache.Get(key2));
            cache.Put(key1, "carbon");
            cache.Put(key2, "oxygen");
            ISet<String> keySet = new HashSet<String>();
            keySet.Add(key1);
            keySet.Add(key2);
            IDictionary<String,String> d = cache.GetAll(keySet);
            Assert.AreEqual(d[key1], cache.Get(key1));
            Assert.AreEqual(d[key2], cache.Get(key2));
            Assert.AreEqual(d[key1], "carbon");
            Assert.AreEqual(d[key2], "oxygen");
        }



        [Test]
        public void GetVersionedTest()
        {
            String key = UniqueKey.NextKey();

            cache.Put(key, "uranium");
            IVersionedValue<String> previous = cache.GetVersioned(key);
            cache.Put(key, "rubidium");

            IVersionedValue<String> current = cache.GetVersioned(key);
            Assert.AreEqual("rubidium", current.GetValue());

            Assert.AreNotEqual(previous.GetVersion(), current.GetVersion());
        }

        [Test]
        public void GetWithMetadataImmortalTest()
        {
            String key = UniqueKey.NextKey();

            /* Created immortal entry. */
            cache.Put(key, "uranium");
            IMetadataValue<String> metadata = cache.GetWithMetadata(key);
            Assert.AreEqual("uranium", metadata.GetValue());

            Assert.IsTrue(metadata.GetLifespan() < 0);
            Assert.AreEqual(-1, metadata.GetCreated());

            Assert.IsTrue(metadata.GetMaxIdle() < 0);
            Assert.AreEqual(-1, metadata.GetLastUsed());
        }

        [Test]
        public void GetWithMetadataTest()
        {
            String key = UniqueKey.NextKey();

            /* Created with lifespan/maxidle. */
            cache.Put(key, "rubidium", 60, TimeUnit.MINUTES, 30, TimeUnit.MINUTES);

            IMetadataValue<String> metadata = cache.GetWithMetadata(key);
            Assert.AreEqual("rubidium", metadata.GetValue());

            Assert.AreEqual(3600, metadata.GetLifespan());
            Assert.AreNotEqual(0, metadata.GetCreated());

            Assert.AreEqual(1800, metadata.GetMaxIdle());
            Assert.AreNotEqual(0, metadata.GetLastUsed());
        }

        [Test]
        public void GetBulkTest()
        {
            String key1 = UniqueKey.NextKey();
            String key2 = UniqueKey.NextKey();
            String key3 = UniqueKey.NextKey();

            cache.Put(key1, "hydrogen");
            cache.Put(key2, "helium");
            cache.Put(key3, "lithium");

            IDictionary<String, String> data;

            data = cache.GetBulk();
            Assert.AreEqual("hydrogen", data[key1]);
            Assert.AreEqual("helium", data[key2]);
            Assert.AreEqual("lithium", data[key3]);

            data = cache.GetBulk(2);
            Assert.AreEqual(data.Count, 2);
        }

        [Test]
        public void PutTest()
        {
            String key1 = UniqueKey.NextKey();
            String key2 = UniqueKey.NextKey();
            ulong initialSize = cache.Size();

            cache.Put(key1, "boron");
            Assert.AreEqual(initialSize + 1, cache.Size());
            Assert.AreEqual("boron", cache.Get(key1));

            cache.Put(key2, "chlorine");
            Assert.AreEqual(initialSize + 2, cache.Size());
            Assert.AreEqual("chlorine", cache.Get(key2));
        }

        [Test]
        public void PutAllTest()
        {
            ulong initialSize = cache.Size();

            String key1 = UniqueKey.NextKey();
            String key2 = UniqueKey.NextKey();
            String key3 = UniqueKey.NextKey();

            Dictionary<String, String> map = new Dictionary<String, String>();
            map.Add(key1, "v1");
            map.Add(key2, "v2");
            map.Add(key3, "v3");

            cache.PutAll(map);
            Assert.AreEqual(initialSize + 3, cache.Size());
            Assert.AreEqual("v1", cache.Get(key1));
            Assert.AreEqual("v2", cache.Get(key2));
            Assert.AreEqual("v3", cache.Get(key3));
        }

        [Test]
        public void ContainsKeyTest()
        {
            String key = UniqueKey.NextKey();

            Assert.IsFalse(cache.ContainsKey(key));
            cache.Put(key, "oxygen");
            Assert.IsTrue(cache.ContainsKey(key));
        }

        [Test]
        public void ContainsValueTest()
        {
            try
            {
                cache.ContainsValue("key");
                Assert.Fail("Should throw an unsupported op exception for now.");
            }
            catch (Infinispan.HotRod.Exceptions.UnsupportedOperationException)
            {
            }
        }

        [Test]
        public void ReplaceWithVersionTest()
        {
            String key = UniqueKey.NextKey();

            cache.Put(key, "bromine");
            ulong version = (ulong)cache.GetVersioned(key).GetVersion();
            cache.Put(key, "hexane");
            bool response = cache.ReplaceWithVersion(key, "barium", version);
            Assert.IsFalse(response);
            Assert.AreEqual("hexane", cache.Get(key));

            cache.Put(key, "oxygen");
            ulong newVersion = (ulong)cache.GetVersioned(key).GetVersion();
            Assert.AreNotEqual(newVersion, version);
            Assert.IsTrue(cache.ReplaceWithVersion(key, "barium", newVersion));
            Assert.AreEqual("barium", cache.Get(key));
        }

        [Test]
        public void RemoveTest()
        {
            String key = UniqueKey.NextKey();

            cache.Put(key, "bromine");
            Assert.IsTrue(cache.ContainsKey(key));
            cache.Remove(key);
            Assert.IsFalse(cache.ContainsKey(key));
        }

        [Test]
        public void RemoveWithVersionTest()
        {
            String key = UniqueKey.NextKey();

            cache.Put(key, "bromine");
            ulong version = (ulong)cache.GetVersioned(key).GetVersion();

            cache.Put(key, "hexane");
            Assert.IsFalse(cache.RemoveWithVersion(key, version));

            version = (ulong)cache.GetVersioned(key).GetVersion();
            Assert.IsTrue(cache.RemoveWithVersion(key, version));
            Assert.IsNull(cache.Get(key));
        }

        [Test]
        public void ClearTest()
        {
            String key1 = UniqueKey.NextKey();
            String key2 = UniqueKey.NextKey();

            cache.Put(key1, "hydrogen");
            cache.Put(key2, "helium");
            Assert.IsFalse(cache.IsEmpty());

            cache.Clear();

            Assert.IsNull(cache.Get(key1));
            Assert.IsNull(cache.Get(key2));

            Assert.AreEqual(0, cache.Size());
            Assert.IsTrue(cache.IsEmpty());
        }

        [Test]
        public void DefaultValueForForceReturnValueTest()
        {
            String key = UniqueKey.NextKey();

            Assert.IsNull(cache.Put(key, "v1"));
            Assert.IsNull(cache.Put(key, "v2"));

            Assert.IsNull(cache.Remove(key));

            Assert.IsNull(cache.Replace(key, "v3"));
        }

        [Test]
        public void EntrySetTest()
        {
            try
            {
                cache.EntrySet();
                Assert.Fail("Should throw an unsupported op exception for now.");
            }
            catch (Infinispan.HotRod.Exceptions.UnsupportedOperationException)
            {
            }
        }

        [Test]
        public void KeySetTest()
        {
            String key1 = UniqueKey.NextKey();
            String key2 = UniqueKey.NextKey();
            String key3 = UniqueKey.NextKey();

            cache.Clear();
            cache.Put(key1, "v1");
            cache.Put(key2, "v2");
            cache.Put(key3, "v3");

            ISet<String> keys = cache.KeySet();
            Assert.AreEqual(3, keys.Count);
            Assert.IsTrue(keys.Contains(key1));
            Assert.IsTrue(keys.Contains(key2));
            Assert.IsTrue(keys.Contains(key3));
        }

        [Test]
        public void ValuesTest()
        {
            try
            {
                cache.Values();
                Assert.Fail("Should throw an unsupported op exception for now.");
            }
            catch (Infinispan.HotRod.Exceptions.UnsupportedOperationException)
            {
            }
        }

        [Test]
        public void WithFlagsTest()
        {
            String key1 = UniqueKey.NextKey();

            Assert.IsNull(cache.Put(key1, "v1"));
            Assert.IsNull(cache.Put(key1, "v2"));

            Flags flags = Flags.FORCE_RETURN_VALUE | Flags.DEFAULT_LIFESPAN | Flags.DEFAULT_MAXIDLE;
            Assert.AreEqual("v2", cache.WithFlags(flags).Put(key1, "v3"));
            Assert.IsNull(cache.Put(key1, "v4"));

            //TODO: requires changes to the server configuration file to configure default expiration
            // IMetadataValue<String> metadata = cache.GetWithMetadata(key1);
            // Assert.IsTrue(metadata.GetLifespan() > 0);
            // Assert.IsTrue(metadata.GetMaxIdle() > 0);
        }

        [Test]
        public void StatTest()
        {
            ServerStatistics stats;

            /* Gather the initial stats. */
            stats = cache.Stats();
            int initialTimeSinceStart = stats.GetIntStatistic(ServerStatistics.TIME_SINCE_START);
            int initialEntries = stats.GetIntStatistic(ServerStatistics.CURRENT_NR_OF_ENTRIES);
            int initialTotalEntries = stats.GetIntStatistic(ServerStatistics.TOTAL_NR_OF_ENTRIES);
            int initialStores = stats.GetIntStatistic(ServerStatistics.STORES);
            int initialRetrievals = stats.GetIntStatistic(ServerStatistics.RETRIEVALS);
            int initialHits = stats.GetIntStatistic(ServerStatistics.HITS);
            int initialMisses = stats.GetIntStatistic(ServerStatistics.MISSES);
            int initialRemoveHits = stats.GetIntStatistic(ServerStatistics.REMOVE_HITS);
            int initialRemoveMisses = stats.GetIntStatistic(ServerStatistics.REMOVE_MISSES);

            /* Check that all are present. */
            Assert.IsTrue(initialTimeSinceStart >= 0);
            Assert.IsTrue(initialEntries >= 0);
            Assert.IsTrue(initialTotalEntries >= 0);
            Assert.IsTrue(initialStores >= 0);
            Assert.IsTrue(initialRetrievals >= 0);
            Assert.IsTrue(initialHits >= 0);
            Assert.IsTrue(initialMisses >= 0);
            Assert.IsTrue(initialRemoveHits >= 0);
            Assert.IsTrue(initialRemoveMisses >= 0);

            /* Add 3 key/value pairs. */
            String key1 = UniqueKey.NextKey();
            String key2 = UniqueKey.NextKey();
            String key3 = UniqueKey.NextKey();

            cache.Put(key1, "v");
            cache.Put(key2, "v");
            cache.Put(key3, "v");

            stats = cache.Stats();
            Assert.AreEqual(initialEntries + 3, stats.GetIntStatistic(ServerStatistics.CURRENT_NR_OF_ENTRIES));
            Assert.AreEqual(initialTotalEntries + 3, stats.GetIntStatistic(ServerStatistics.TOTAL_NR_OF_ENTRIES));
            Assert.AreEqual(initialStores + 3, stats.GetIntStatistic(ServerStatistics.STORES));

            /* Get hit/misses. */
            cache.Get(key1);
            cache.Get(key2);
            cache.Get(UniqueKey.NextKey());

            stats = cache.Stats();
            Assert.AreEqual(initialRetrievals + 3, stats.GetIntStatistic(ServerStatistics.RETRIEVALS));
            Assert.AreEqual(initialHits + 2, stats.GetIntStatistic(ServerStatistics.HITS));
            Assert.AreEqual(initialMisses + 1, stats.GetIntStatistic(ServerStatistics.MISSES));

            /* Remove hit/misses. */
            cache.Remove(key3);
            cache.Remove(UniqueKey.NextKey());
            cache.Remove(UniqueKey.NextKey());
            cache.Remove(UniqueKey.NextKey());

            stats = cache.Stats();
            Assert.AreEqual(initialRemoveHits + 1, stats.GetIntStatistic(ServerStatistics.REMOVE_HITS));
            Assert.AreEqual(initialRemoveMisses + 3, stats.GetIntStatistic(ServerStatistics.REMOVE_MISSES));

            /* Clear the cache. */
            cache.Clear();

            stats = cache.Stats();
            Assert.AreEqual(0, stats.GetIntStatistic(ServerStatistics.CURRENT_NR_OF_ENTRIES));
            Assert.AreEqual(initialTotalEntries + 3, stats.GetIntStatistic(ServerStatistics.TOTAL_NR_OF_ENTRIES));
        }
    }
}
