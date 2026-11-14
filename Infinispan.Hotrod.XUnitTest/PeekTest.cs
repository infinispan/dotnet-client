using System;
using System.Threading.Tasks;
using Infinispan.Hotrod.Tests.Util;
using Xunit;

namespace Infinispan.Hotrod.XUnitTest
{
    public class PeekTestFixture : IAsyncLifetime
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
    public class PeekTest : IClassFixture<PeekTestFixture>
    {
        private readonly Cache<string, string> _cache;

        public PeekTest(PeekTestFixture fixture)
        {
            _cache = fixture.cache;
        }

        [Fact]
        public async Task PeekReturnsValueAndMetadata()
        {
            String key = UniqueKey.NextKey();
            await _cache.Put(key, "value", new ExpirationTime { Unit = TimeUnit.MINUTES, Value = 10 }, new ExpirationTime { Unit = TimeUnit.MINUTES, Value = 5 });

            var vwm = await _cache.Peek(key);

            Assert.NotNull(vwm);
            Assert.Equal("value", vwm.Value);
            Assert.Equal(600, vwm.Lifespan);
            Assert.Equal(300, vwm.MaxIdle);
        }

        [Fact]
        public async Task PeekNonExistentKey()
        {
            String key = UniqueKey.NextKey();

            var vwm = await _cache.Peek(key);

            Assert.Null(vwm);
        }

        [Fact]
        public async Task DoesNotResetMaxIdle()
        {
            String peekedKey = UniqueKey.NextKey();
            String accessedKey = UniqueKey.NextKey();
            ExpirationTime maxIdle = new ExpirationTime { Unit = TimeUnit.SECONDS, Value = 2 };

            await _cache.Put(peekedKey, "value", null, maxIdle);
            await _cache.Put(accessedKey, "value", null, maxIdle);

            await Task.Delay(TimeSpan.FromSeconds(1));

            var vwm = await _cache.Peek(peekedKey);
            Assert.NotNull(vwm);
            Assert.Equal("value", vwm.Value);

            String accessedValue = await _cache.Get(accessedKey);
            Assert.Equal("value", accessedValue);

            await Task.Delay(TimeSpan.FromMilliseconds(1600));

            Assert.Null(await _cache.Get(peekedKey));
            Assert.Equal("value", await _cache.Get(accessedKey));
        }
    }
}
