using System;
using System.Threading.Tasks;
using Infinispan.Hotrod.Tests.Util;
using Xunit;

namespace Infinispan.Hotrod.XUnitTest
{
    public class NearCacheTestFixture : IAsyncLifetime
    {
        private InfinispanContainer _container;
        public InfinispanClient infinispan = new InfinispanClient();

        public async Task InitializeAsync()
        {
            _container = new InfinispanContainer("infinispan-noauth.xml");
            await _container.StartAsync();
            infinispan.AddHost(_container.Host, _container.Port);
            infinispan.Version = 0x1f;
            infinispan.ForceReturnValue = false;
            infinispan.ClientIntelligence = 0x01;
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }
    }

    [Collection("MainSequence")]
    public class NearCacheTest : IClassFixture<NearCacheTestFixture>
    {
        private readonly NearCacheTestFixture _fixture;

        public NearCacheTest(NearCacheTestFixture fixture)
        {
            _fixture = fixture;
        }

        private Cache<string, string> NewCache()
        {
            return _fixture.infinispan.NewCache(new StringMarshaller(), new StringMarshaller(), "default");
        }

        [Fact]
        public async Task NearCacheHitTest()
        {
            var cache = NewCache();
            await cache.EnableNearCache(1000);
            string key = UniqueKey.NextKey();
            await cache.Put(key, "value1");

            await cache.Get(key);
            await cache.Get(key);

            var stats = cache.NearCacheStats;
            Assert.True(stats.Hits >= 1, $"Expected at least 1 hit, got {stats.Hits}");
            Assert.True(stats.Misses >= 1, $"Expected at least 1 miss, got {stats.Misses}");
        }

        [Fact]
        public async Task NearCacheInvalidationOnPutTest()
        {
            var cache = NewCache();
            await cache.EnableNearCache(1000);
            string key = UniqueKey.NextKey();
            await cache.Put(key, "value1");

            Assert.Equal("value1", await cache.Get(key));

            await cache.Put(key, "value2");

            Assert.Equal("value2", await cache.Get(key));
        }

        [Fact]
        public async Task NearCacheInvalidationOnRemoveTest()
        {
            var cache = NewCache();
            await cache.EnableNearCache(1000);
            string key = UniqueKey.NextKey();
            await cache.Put(key, "value1");

            Assert.Equal("value1", await cache.Get(key));

            await cache.Remove(key);

            Assert.Null(await cache.Get(key));
        }

        [Fact]
        public async Task NearCacheClearTest()
        {
            var cache = NewCache();
            await cache.EnableNearCache(1000);
            string key1 = UniqueKey.NextKey();
            string key2 = UniqueKey.NextKey();
            await cache.Put(key1, "a");
            await cache.Put(key2, "b");

            await cache.Get(key1);
            await cache.Get(key2);

            Assert.True(cache.NearCacheStats.Size >= 2);

            await cache.Clear();

            Assert.Equal(0, cache.NearCacheStats.Size);
        }

        [Fact]
        public async Task NearCacheEvictionTest()
        {
            var cache = NewCache();
            await cache.EnableNearCache(5);

            for (int i = 0; i < 10; i++)
            {
                string key = $"evict-{UniqueKey.NextKey()}";
                await cache.Put(key, $"value-{i}");
                await cache.Get(key);
            }

            Assert.True(cache.NearCacheStats.Size <= 5,
                $"Near cache size {cache.NearCacheStats.Size} exceeds max of 5");
        }

        [Fact]
        public void NearCacheDisabledByDefaultTest()
        {
            var cache = NewCache();
            Assert.Null(cache.NearCacheStats);
        }

        [Fact]
        public async Task NearCacheDoubleEnableThrowsTest()
        {
            var cache = NewCache();
            await cache.EnableNearCache(100);
            await Assert.ThrowsAsync<InvalidOperationException>(() => cache.EnableNearCache(100));
        }
    }
}
