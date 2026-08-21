using System;
using System.Threading.Tasks;
using Infinispan.Hotrod;
using Infinispan.Hotrod.Tests.Util;
using Xunit;

namespace Infinispan.Hotrod.XUnitTest
{
    [Collection("Sequential")]
    public class FailoverCacheTestFixture : IAsyncLifetime
    {
        public InfinispanContainer container1;
        public InfinispanContainer container2;
        public Cache<string, string> cache1;
        public Cache<string, string> cache2;
        public InfinispanClient infinispan1 = new InfinispanClient();
        public InfinispanClient infinispan2 = new InfinispanClient();

        public async Task InitializeAsync()
        {
            container1 = new InfinispanContainer("infinispan-noauth.xml");
            container2 = new InfinispanContainer("infinispan-noauth.xml");
            await container1.StartAsync();
            await container2.StartAsync();

            infinispan1.AddHost(container1.Host, container1.Port);
            infinispan1.AddHost("nyc", container2.Host, container2.Port);
            infinispan1.Version = ProtocolVersion.Version31;
            infinispan1.ForceReturnValue = false;
            infinispan1.ClientIntelligence = ClientIntelligence.Basic;
            cache1 = infinispan1.NewCache(new StringMarshaller(), new StringMarshaller(), "default");

            infinispan2.AddHost(container2.Host, container2.Port);
            infinispan2.AddHost("lon", container1.Host, container1.Port);
            infinispan2.Version = ProtocolVersion.Version31;
            infinispan2.ForceReturnValue = false;
            infinispan2.ClientIntelligence = ClientIntelligence.Basic;
            cache2 = infinispan2.NewCache(new StringMarshaller(), new StringMarshaller(), "default");
        }

        public async Task DisposeAsync()
        {
            await container1.DisposeAsync();
            await container2.DisposeAsync();
        }
    }
    [Collection("MainSequence")]
    public class FailoverCacheTest : IClassFixture<FailoverCacheTestFixture>
    {
        private readonly FailoverCacheTestFixture _fixture;
        private Cache<string, string> _cache1;
        private Cache<string, string> _cache2;
        private InfinispanClient _infinispan1;
        private InfinispanClient _infinispan2;
        public FailoverCacheTest(FailoverCacheTestFixture fixture)
        {
            _fixture = fixture;
            _cache1 = _fixture.cache1;
            _cache2 = _fixture.cache2;
            _infinispan1 = _fixture.infinispan1;
            _infinispan2 = _fixture.infinispan2;
            _fixture.infinispan1.SwitchCluster("DEFAULT_CLUSTER");
            _fixture.infinispan2.SwitchCluster("DEFAULT_CLUSTER");
        }

        [Fact]
        public async Task verifyHotRodServersTest()
        {
            Assert.NotNull(await _cache1.Ping());
            Assert.NotNull(await _cache2.Ping());
        }

        [Fact]
        public async Task multipleClustersTest()
        {
            String key = UniqueKey.NextKey();
            await _cache1.Put(key, "valueCache1");
            await _cache2.Put(key, "valueCache2");
            Assert.Equal("valueCache1", await _cache1.Get(key));
            Assert.Equal("valueCache2", await _cache2.Get(key));
        }
        [Fact]
        public async Task manualClusterSwitchTest()
        {
            String key = UniqueKey.NextKey();
            await _cache1.Put(key, "valueCache1");
            await _cache2.Put(key, "valueCache2");
            Assert.Equal("valueCache1", await _cache1.Get(key));
            _infinispan1.SwitchCluster("nyc");
            Assert.Equal("valueCache2", await _cache1.Get(key));
            Assert.Equal("valueCache2", await _cache2.Get(key));
            _infinispan2.SwitchCluster("lon");
            Assert.Equal("valueCache1", await _cache2.Get(key));
        }

        [Fact(Skip = "Client failover recovery after container restart needs investigation")]
        public async Task ClusterSwitchOnFaultTest()
        {
            await Task.CompletedTask;
        }
    }
}
