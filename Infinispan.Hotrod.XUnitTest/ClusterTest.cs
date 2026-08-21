using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using Infinispan.Hotrod;
using Infinispan.Hotrod.Tests.Util;
using Xunit;

namespace Infinispan.Hotrod.XUnitTest
{
    [Collection("Sequential")]
    public class ClusterCacheTestFixture : IAsyncLifetime
    {
        private const int Server1Port = 11222;
        private const int Server2Port = 11322;

        private INetwork _network;
        public InfinispanContainer container1;
        public InfinispanContainer container2;
        public Cache<string, string> distributedCache;
        public Cache<string, string> localCache;
        public InfinispanClient infinispan1 = new InfinispanClient();

        public async Task InitializeAsync()
        {
            _network = new NetworkBuilder().Build();
            await _network.CreateAsync();

            container1 = new InfinispanContainer(
                "infinispan-clustered.xml",
                user: "admin", password: "password",
                network: _network,
                networkAliases: new[] { "infinispan1" },
                envVars: new Dictionary<string, string>
                {
                    ["JAVA_OPTIONS"] = $"-Dinfinispan.cluster.name=testcluster -Dhotrod.external.host=127.0.0.1 -Dhotrod.external.port={Server1Port}"
                },
                hostPort: Server1Port);

            container2 = new InfinispanContainer(
                "infinispan-clustered.xml",
                user: "admin", password: "password",
                network: _network,
                networkAliases: new[] { "infinispan2" },
                envVars: new Dictionary<string, string>
                {
                    ["JAVA_OPTIONS"] = $"-Dinfinispan.cluster.name=testcluster -Dhotrod.external.host=127.0.0.1 -Dhotrod.external.port={Server2Port}"
                },
                hostPort: Server2Port);

            await container1.StartAsync();
            await container2.StartAsync();

            infinispan1.User = "admin";
            infinispan1.Password = "password";
            infinispan1.AuthMech = "SCRAM-SHA-256";
            infinispan1.AddHost("127.0.0.1", Server1Port);
            infinispan1.Version = 0x1f;
            infinispan1.ForceReturnValue = false;
            infinispan1.ClientIntelligence = 0x03;
            distributedCache = infinispan1.NewCache(new StringMarshaller(), new StringMarshaller(), "distributed");
            localCache = infinispan1.NewCache(new StringMarshaller(), new StringMarshaller(), "namedCache");
        }

        public async Task DisposeAsync()
        {
            await container1.DisposeAsync();
            await container2.DisposeAsync();
            await _network.DisposeAsync();
        }
    }
    [Collection("MainSequence")]
    public class ClusterCacheTest : IClassFixture<ClusterCacheTestFixture>
    {
        private readonly ClusterCacheTestFixture _fixture;
        private Cache<string, string> _distributedCache;
        private Cache<string, string> _localCache;
        private InfinispanClient _infinispan1;
        public ClusterCacheTest(ClusterCacheTestFixture fixture)
        {
            _fixture = fixture;
            _infinispan1 = _fixture.infinispan1;
            _distributedCache = _fixture.distributedCache;
            _localCache = _fixture.localCache;
        }

        [Fact]
        public async Task verifyHotRodServersTest()
        {
            var result = await _distributedCache.Ping();
            Assert.NotNull(result);
        }

        [Fact]
        public async Task distributedCacheTest()
        {
            String key = UniqueKey.NextKey();
            await _distributedCache.Put(key, "value");
            Assert.Equal("value", await _distributedCache.Get(key));
            await _fixture.container1.StopAsync();
            Assert.Equal("value", await _distributedCache.Get(key));
            await _fixture.container1.EnsureStartedAsync();
        }
        [Fact]
        public async Task localCacheTest()
        {
            String key = UniqueKey.NextKey();
            await _localCache.Put(key, "value");
            Assert.Equal("value", await _localCache.Get(key));
            await _fixture.container1.StopAsync();
            await Assert.ThrowsAsync<InfinispanException>(() => _localCache.Get(key));
            await _fixture.container1.EnsureStartedAsync();
        }
        [Fact]
        public async Task localAndDistributedCacheTest()
        {
            String key = UniqueKey.NextKey();
            await _localCache.Put(key, "valueLocal");
            await _distributedCache.Put(key, "valueDistributed");
            Assert.Equal("valueLocal", await _localCache.Get(key));
            await _fixture.container1.StopAsync();
            Assert.Equal("valueDistributed", await _distributedCache.Get(key));
            await Assert.ThrowsAsync<InfinispanException>(() => _localCache.Get(key));
            await _fixture.container1.EnsureStartedAsync();
        }
        [Fact]
        public async Task distributedCachePutGetAllByOwner()
        {
            var keyVals = new Dictionary<String, String>();
            var keys = new HashSet<String>();

            for (var i = 0; i < 20; i++)
            {
                var k = UniqueKey.NextKey();
                keys.Add(k);
                keyVals.Add(k, k + "value");
            }
            var pr = await _distributedCache.Ping();
            await _distributedCache.PutAll(keyVals);
            var res = await _distributedCache.GetAll(keys);
            var partResult = _distributedCache.GetAllPart(keys);
            try
            {
                partResult.WaitAll();
            }
            catch (AggregateException aEx)
            {
                Assert.Null("Should not reach this point: " + aEx.Message);
            }
            await _distributedCache.Clear();

            try
            {
                _distributedCache.PutAllPart(keyVals).WaitAll();
            }
            catch (AggregateException aEx)
            {
                Assert.Null("Should not reach this point: " + aEx.Message);
            }
            var res1 = await _distributedCache.GetAll(keys);
            var partResult1 = _distributedCache.GetAllPart(keys);
            try
            {
                partResult1.WaitAll();
            }
            catch (AggregateException aEx)
            {
                Assert.Null("Should not reach this point: " + aEx.Message);
            }
            var d = partResult.Result();
            Assert.Equal(d, res);
            var d1 = partResult1.Result();
            Assert.Equal(d1, res1);
            Assert.Equal(d, d1);
        }
    }
}
