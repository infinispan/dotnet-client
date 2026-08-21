using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infinispan.Hotrod.Tests.Util;
using Xunit;

namespace Infinispan.Hotrod.XUnitTest
{
    public class DefaultCacheTestFixture : IAsyncLifetime
    {
        private InfinispanContainer _container;
        public Cache<string, string> cache;
        public InfinispanClient infinispan = new InfinispanClient();

        public async Task InitializeAsync()
        {
            _container = new InfinispanContainer("infinispan-noauth.xml");
            await _container.StartAsync();
            infinispan.AddHost(_container.Host, _container.Port);
            infinispan.Version = ProtocolVersion.Version31;
            infinispan.ForceReturnValue = false;
            infinispan.ClientIntelligence = ClientIntelligence.Basic;
            cache = infinispan.NewCache(new StringMarshaller(), new StringMarshaller(), "default");
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }
    }
    [Collection("MainSequence")]
    public class DefaultCacheTest : IClassFixture<DefaultCacheTestFixture>
    {
        private readonly DefaultCacheTestFixture _fixture;
        private Cache<string, string> _cache;
        private InfinispanClient _infinispan;
        public DefaultCacheTest(DefaultCacheTestFixture fixture)
        {
            _fixture = fixture;
            _cache = _fixture.cache;
            _infinispan = _fixture.infinispan;
        }

        [Fact]
        public async Task WrongConnectionTest()
        {
            var infinispan = new InfinispanClient();
            infinispan.AddHost("127.0.0.1", 19999);
            infinispan.Version = ProtocolVersion.Version31;
            infinispan.ForceReturnValue = false;
            infinispan.ClientIntelligence = ClientIntelligence.Basic;
            var cache = infinispan.NewCache(new StringMarshaller(), new StringMarshaller(), "default");
            String key = UniqueKey.NextKey();
            var excpt = await Assert.ThrowsAsync<InfinispanException>(() => cache.Get(key));
            Assert.Equal("Infinispan server is not available", excpt.Message);
        }

        [Fact]
        public async Task WrongCacheNameTest()
        {
            var cache = _infinispan.NewCache(new StringMarshaller(), new StringMarshaller(), "nonExistent");
            String key = UniqueKey.NextKey();
            var excpt = await Assert.ThrowsAsync<InfinispanException>(() => cache.Get(key));
            Assert.Equal("org.infinispan.server.hotrod.CacheNotFoundException: Cache with name 'nonExistent' not found amongst the configured caches", excpt.Message);
        }

        [Fact]
        public void NameTest()
        {
            Assert.NotNull(_cache.Name);
        }

        [Fact]
        public void VersionTest()
        {
            Assert.NotEqual((ProtocolVersion)0, _cache.Cluster.Version);
        }

        // TODO: Verify if GetProtocolVersion method is needed
        // [Fact]
        // public void ProtocolVersionTest()
        // {
        //     Assert.NotNull(_cache.GetProtocolVersion());
        // }

        [Fact]
        public async Task GetTest()
        {
            String key = UniqueKey.NextKey();

            Assert.Null(await _cache.Get(key));
            await _cache.Put(key, "carbon");
            Assert.Equal("carbon", await _cache.Get(key));
        }

        [Fact]
        public async Task PutTest()
        {
            String key1 = UniqueKey.NextKey();
            String key2 = UniqueKey.NextKey();
            Int32 initialSize = await _cache.Size();

            await _cache.Put(key1, "boron");
            Assert.Equal(initialSize + 1, await _cache.Size());
            Assert.Equal("boron", await _cache.Get(key1));

            await _cache.Put(key2, "chlorine");
            Assert.Equal(initialSize + 2, await _cache.Size());
            Assert.Equal("chlorine", await _cache.Get(key2));
        }

        [Fact]
        public async Task ContainsKeyTest()
        {
            String key = UniqueKey.NextKey();
            Assert.False(await _cache.ContainsKey(key));
            await _cache.Put(key, "oxygen");
            Assert.True(await _cache.ContainsKey(key));
        }


        [Fact]
        public async Task RemoveTest()
        {
            String key = UniqueKey.NextKey();
            await _cache.Put(key, "bromine");
            Assert.True(await _cache.ContainsKey(key));
            await _cache.Remove(key);
            Assert.False(await _cache.ContainsKey(key));
        }

        [Fact]
        public async Task ClearTest()
        {
            String key1 = UniqueKey.NextKey();
            String key2 = UniqueKey.NextKey();

            await _cache.Put(key1, "hydrogen");
            await _cache.Put(key2, "helium");
            Assert.False(await _cache.IsEmpty());

            await _cache.Clear();

            Assert.Null(await _cache.Get(key1));
            Assert.Null(await _cache.Get(key2));

            Assert.Equal(0, await _cache.Size());
            Assert.True(await _cache.IsEmpty());
        }

        [Fact]
        public async Task GetWithVersionTest()
        {
            String key = UniqueKey.NextKey();
            await _cache.Put(key, "uranium");
            ValueWithVersion<string> previous = await _cache.GetWithVersion(key);
            await _cache.Put(key, "rubidium");
            ValueWithVersion<string> current = await _cache.GetWithVersion(key);
            Assert.Equal("rubidium", current.Value);
            Assert.NotEqual(previous.Version, current.Version);
        }

        [Fact]
        public async Task GetWithMetadataTest()
        {
            String key = UniqueKey.NextKey();

            /* Created with lifespan/maxidle. */
            await _cache.Put(key, "rubidium", new ExpirationTime { Value = 60, Unit = TimeUnit.MINUTES }, new ExpirationTime { Value = 30, Unit = TimeUnit.MINUTES });

            ValueWithMetadata<string> metadata = await _cache.GetWithMetadata(key);
            Assert.Equal("rubidium", metadata.Value);

            Assert.Equal((Int32)3600, metadata.Lifespan);
            Assert.NotEqual((Int64)0, metadata.Created);

            Assert.Equal((Int32)1800, metadata.MaxIdle);
            Assert.NotEqual((Int64)0, metadata.LastUsed);
        }

        [Fact]
        public async Task GetWithMetadataImmortalTest()
        {
            String key = UniqueKey.NextKey();

            /* Created immortal entry. */
            await _cache.Put(key, "uranium");
            ValueWithMetadata<string> metadata = await _cache.GetWithMetadata(key);
            Assert.Equal("uranium", metadata.Value);

            Assert.True(metadata.Lifespan < 0);
            Assert.Equal(-1, metadata.Created);

            Assert.True(metadata.MaxIdle < 0);
            Assert.Equal(-1, metadata.LastUsed);
        }

        [Fact]
        public async Task StatTest()
        {
            ServerStatistics stats;

            /* Gather the initial stats. */
            stats = await _cache.Stats();
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
            Assert.True(initialTimeSinceStart >= 0);
            Assert.True(initialTotalEntries >= 0);
            Assert.True(initialStores >= 0);
            Assert.True(initialRetrievals >= 0);
            Assert.True(initialHits >= 0);
            Assert.True(initialMisses >= 0);
            Assert.True(initialRemoveHits >= 0);
            Assert.True(initialRemoveMisses >= 0);

            /* Add 3 key/value pairs. */
            String key1 = UniqueKey.NextKey();
            String key2 = UniqueKey.NextKey();
            String key3 = UniqueKey.NextKey();

            await _cache.Put(key1, "v");
            await _cache.Put(key2, "v");
            await _cache.Put(key3, "v");

            stats = await _cache.Stats();
            Assert.Equal(initialStores + 3, stats.GetIntStatistic(ServerStatistics.STORES));

            /* Get hit/misses. */
            await _cache.Get(key1);
            await _cache.Get(key2);
            await _cache.Get(UniqueKey.NextKey());

            stats = await _cache.Stats();
            Assert.Equal(initialRetrievals + 3, stats.GetIntStatistic(ServerStatistics.RETRIEVALS));
            Assert.Equal(initialHits + 2, stats.GetIntStatistic(ServerStatistics.HITS));
            Assert.Equal(initialMisses + 1, stats.GetIntStatistic(ServerStatistics.MISSES));

            /* Remove hit/misses. */
            await _cache.Remove(key3);
            await _cache.Remove(UniqueKey.NextKey());
            await _cache.Remove(UniqueKey.NextKey());
            await _cache.Remove(UniqueKey.NextKey());

            stats = await _cache.Stats();
            Assert.Equal(initialRemoveHits + 1, stats.GetIntStatistic(ServerStatistics.REMOVE_HITS));
            Assert.Equal(initialRemoveMisses + 3, stats.GetIntStatistic(ServerStatistics.REMOVE_MISSES));
        }

        [Fact]
        public async Task ReplaceWithVersionTest()
        {
            String key = UniqueKey.NextKey();
            await _cache.Put(key, "bromine");
            long version = (await _cache.GetWithVersion(key)).Version;
            await _cache.Put(key, "hexane");
            bool response = await _cache.ReplaceWithVersion(key, "barium", version);
            Assert.False(response);
            Assert.Equal("hexane", await _cache.Get(key));

            await _cache.Put(key, "oxygen");
            long newVersion = (await _cache.GetWithVersion(key)).Version;
            Assert.NotEqual(newVersion, version);
            Assert.True(await _cache.ReplaceWithVersion(key, "barium", newVersion));
            Assert.Equal("barium", await _cache.Get(key));
        }

        [Fact]
        public async Task RemoveWithVersionTest()
        {
            String key = UniqueKey.NextKey();

            await _cache.Put(key, "bromine");
            long version = (await _cache.GetWithVersion(key)).Version;

            await _cache.Put(key, "hexane");
            Assert.False((await _cache.RemoveWithVersion(key, version)).Removed);

            version = (long)(await _cache.GetWithVersion(key)).Version;
            Assert.True((await _cache.RemoveWithVersion(key, version)).Removed);
            Assert.Null(await _cache.Get(key));
        }

        [Fact]
        public async Task DefaultValueForForceReturnValueTest()
        {
            String key = UniqueKey.NextKey();
            Assert.Null(await _cache.Put(key, "v1"));
            Assert.Null(await _cache.Put(key, "v2"));
            Assert.Null((await _cache.Remove(key)).PrevValue);
            Assert.Null((await _cache.Replace(key, "v3")).PrevValue);
        }

        [Fact]
        public async Task GetAllTest()
        {
            String key1 = UniqueKey.NextKey();
            String key2 = UniqueKey.NextKey();
            await _cache.Clear();
            Assert.Null(await _cache.Get(key1));
            Assert.Null(await _cache.Get(key2));
            await _cache.Put(key1, "carbon");
            await _cache.Put(key2, "oxygen");
            ISet<String> keySet = new HashSet<String>();
            keySet.Add(key1);
            keySet.Add(key2);
            IDictionary<String, String> d = await _cache.GetAll(keySet);
            Assert.Equal(d[key1], await _cache.Get(key1));
            Assert.Equal(d[key2], await _cache.Get(key2));
            Assert.Equal("carbon", d[key1]);
            Assert.Equal("oxygen", d[key2]);
        }

        [Fact]
        public async Task GetAllByOwnerTest()
        {
            String key1 = UniqueKey.NextKey();
            String key2 = UniqueKey.NextKey();
            await _cache.Clear();
            Assert.Null(await _cache.Get(key1));
            Assert.Null(await _cache.Get(key2));
            await _cache.Put(key1, "carbon");
            await _cache.Put(key2, "oxygen");
            ISet<String> keySet = new HashSet<String>();
            keySet.Add(key1);
            keySet.Add(key2);
            var partResult = _cache.GetAllPart(keySet);
            // Skip this if there's no topology
            if (partResult == null)
                return;
            try
            {
                partResult.WaitAll();
            }
            catch (AggregateException aex)
            {
                Assert.Null(aex);
            }
            var d = partResult.Result();
            Assert.Equal(d[key1], await _cache.Get(key1));
            Assert.Equal(d[key2], await _cache.Get(key2));
            Assert.Equal("carbon", d[key1]);
            Assert.Equal("oxygen", d[key2]);
        }

        [Fact]
        public async Task pingTest()
        {
            var result = await _cache.Ping();
            Assert.NotNull(result);
        }

        // [Test]
        // public void GetBulkTest()
        // {
        //     String key1 = UniqueKey.NextKey();
        //     String key2 = UniqueKey.NextKey();
        //     String key3 = UniqueKey.NextKey();

        //     cache.Put(key1, "hydrogen");
        //     cache.Put(key2, "helium");
        //     cache.Put(key3, "lithium");

        //     IDictionary<String, String> data;

        //     data = cache.GetBulk();
        //     Assert.AreEqual("hydrogen", data[key1]);
        //     Assert.AreEqual("helium", data[key2]);
        //     Assert.AreEqual("lithium", data[key3]);

        //     data = cache.GetBulk(2);
        //     Assert.AreEqual(data.Count, 2);
        // }

        [Fact]
        public async Task PutAllTest()
        {
            int initialSize = await _cache.Size();

            String key1 = UniqueKey.NextKey();
            String key2 = UniqueKey.NextKey();
            String key3 = UniqueKey.NextKey();

            Dictionary<String, String> map = new Dictionary<String, String>();
            map.Add(key1, "v1");
            map.Add(key2, "v2");
            map.Add(key3, "v3");

            await _cache.PutAll(map);
            Assert.Equal(initialSize + 3, await _cache.Size());
            Assert.Equal("v1", await _cache.Get(key1));
            Assert.Equal("v2", await _cache.Get(key2));
            Assert.Equal("v3", await _cache.Get(key3));
        }

        // [Test]
        // public void ContainsValueTest()
        // {
        //     try
        //     {
        //         cache.ContainsValue("key");
        //         Assert.Fail("Should throw an unsupported op exception for now.");
        //     }
        //     catch (Infinispan.Hotrod.Exceptions.UnsupportedOperationException)
        //     {
        //     }
        // }



        [Fact]
        public async Task EntrySetTest()
        {
            String key1 = UniqueKey.NextKey();
            String key2 = UniqueKey.NextKey();
            String key3 = UniqueKey.NextKey();

            await _cache.Clear();
            await _cache.Put(key1, "hydrogen");
            await _cache.Put(key2, "helium");
            await _cache.Put(key3, "lithium");

            var entries = new Dictionary<string, string>();
            await foreach (var entry in _cache.EntrySet())
            {
                entries[entry.Key] = entry.Value;
            }
            Assert.Equal(3, entries.Count);
            Assert.Equal("hydrogen", entries[key1]);
            Assert.Equal("helium", entries[key2]);
            Assert.Equal("lithium", entries[key3]);
        }

        [Fact]
        public async Task KeySetTest()
        {
            String key1 = UniqueKey.NextKey();
            String key2 = UniqueKey.NextKey();
            String key3 = UniqueKey.NextKey();

            await _cache.Clear();
            await _cache.Put(key1, "v1");
            await _cache.Put(key2, "v2");
            await _cache.Put(key3, "v3");

            ISet<String> keys = await _cache.KeySet();
            Assert.Equal(3, keys.Count);
            Assert.True(keys.Contains(key1));
            Assert.True(keys.Contains(key2));
            Assert.True(keys.Contains(key3));
        }

        [Fact]
        public async Task ValuesTest()
        {
            String key1 = UniqueKey.NextKey();
            String key2 = UniqueKey.NextKey();

            await _cache.Clear();
            await _cache.Put(key1, "neon");
            await _cache.Put(key2, "argon");

            var values = new List<string>();
            await foreach (var v in _cache.Values())
            {
                values.Add(v);
            }
            Assert.Equal(2, values.Count);
            Assert.Contains("neon", values);
            Assert.Contains("argon", values);
        }

        [Fact]
        public async Task RetrieveEntriesWithMetadataTest()
        {
            String key1 = UniqueKey.NextKey();

            await _cache.Clear();
            var lifespan = new ExpirationTime { Unit = TimeUnit.SECONDS, Value = 300 };
            await _cache.Put(key1, "krypton", lifespan);

            await foreach (var entry in _cache.RetrieveEntriesWithMetadata())
            {
                Assert.Equal(key1, entry.Key);
                Assert.Equal("krypton", entry.Value.Value);
                Assert.Equal(300, entry.Value.Lifespan);
            }
        }

        [Fact]
        public async Task RetrieveEntriesEmptyCacheTest()
        {
            await _cache.Clear();
            var entries = new List<KeyValuePair<string, string>>();
            await foreach (var entry in _cache.RetrieveEntries())
            {
                entries.Add(entry);
            }
            Assert.Empty(entries);
        }

        // [Test]
        // public void WithFlagsTest()
        // {
        //     String key1 = UniqueKey.NextKey();

        //     Assert.IsNull(cache.Put(key1, "v1"));
        //     Assert.IsNull(cache.Put(key1, "v2"));

        //     Flags flags = Flags.FORCE_RETURN_VALUE | Flags.DEFAULT_LIFESPAN | Flags.DEFAULT_MAXIDLE;
        //     Assert.AreEqual("v2", cache.WithFlags(flags).Put(key1, "v3"));
        //     Assert.IsNull(cache.Put(key1, "v4"));

        //     //TODO: requires changes to the server configuration file to configure default expiration
        //     // IMetadataValue<String> metadata = cache.GetWithMetadata(key1);
        //     // Assert.IsTrue(metadata.GetLifespan() > 0);
        //     // Assert.IsTrue(metadata.GetMaxIdle() > 0);
        // }

    }
}
