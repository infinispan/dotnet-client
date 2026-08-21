using System.Linq;
using System.Threading.Tasks;
using Infinispan.Hotrod;
using Infinispan.Hotrod.Tests.Util;
using Xunit;

namespace Infinispan.Hotrod.XUnitTest
{
    public class MultimapTestFixture : IAsyncLifetime
    {
        private InfinispanContainer _container;
        public InfinispanClient infinispan;
        private static readonly string[] CacheNames = {
            "mm-putget", "mm-getne", "mm-rmkey", "mm-rment",
            "mm-ck", "mm-cv", "mm-ce", "mm-size"
        };

        public async Task InitializeAsync()
        {
            _container = new InfinispanContainer("infinispan-noauth.xml");
            await _container.StartAsync();

            infinispan = new InfinispanClient();
            infinispan.AddHost(_container.Host, _container.Port);
            infinispan.Version = ProtocolVersion.Version31;
            infinispan.ClientIntelligence = ClientIntelligence.Basic;

            var admin = infinispan.Administration();
            foreach (var name in CacheNames)
            {
                await admin.GetOrCreateCache(name);
            }
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }
    }

    [Collection("MainSequence")]
    public class MultimapTest : IClassFixture<MultimapTestFixture>
    {
        private readonly InfinispanClient _client;

        public MultimapTest(MultimapTestFixture fixture)
        {
            _client = fixture.infinispan;
        }

        private MultimapCache<string, string> NewMultimap(string name)
        {
            return _client.NewMultimap(new StringMarshaller(), new StringMarshaller(), name);
        }

        [Fact]
        public async Task PutAndGetTest()
        {
            var mm = NewMultimap("mm-putget");
            await mm.Put("colors", "red");
            await mm.Put("colors", "blue");
            await mm.Put("colors", "green");

            var values = await mm.Get("colors");
            Assert.Equal(3, values.Count);
            Assert.Contains("red", values);
            Assert.Contains("blue", values);
            Assert.Contains("green", values);
        }

        [Fact]
        public async Task GetNonExistentKeyTest()
        {
            var mm = NewMultimap("mm-getne");
            var values = await mm.Get("missing");
            Assert.Empty(values);
        }

        [Fact]
        public async Task RemoveKeyTest()
        {
            var mm = NewMultimap("mm-rmkey");
            await mm.Put("k", "v1");
            await mm.Put("k", "v2");

            var removed = await mm.RemoveKey("k");
            Assert.True(removed);

            var values = await mm.Get("k");
            Assert.Empty(values);

            var removedAgain = await mm.RemoveKey("k");
            Assert.False(removedAgain);
        }

        [Fact]
        public async Task RemoveEntryTest()
        {
            var mm = NewMultimap("mm-rment");
            await mm.Put("k", "v1");
            await mm.Put("k", "v2");

            var removed = await mm.RemoveEntry("k", "v1");
            Assert.True(removed);

            var values = await mm.Get("k");
            Assert.Single(values);
            Assert.Equal("v2", values[0]);
        }

        [Fact]
        public async Task ContainsKeyTest()
        {
            var mm = NewMultimap("mm-ck");

            Assert.False(await mm.ContainsKey("k"));

            await mm.Put("k", "v");
            Assert.True(await mm.ContainsKey("k"));
        }

        [Fact]
        public async Task ContainsValueTest()
        {
            var mm = NewMultimap("mm-cv");
            await mm.Put("k", "v1");

            Assert.True(await mm.ContainsValue("v1"));
            Assert.False(await mm.ContainsValue("v999"));
        }

        [Fact]
        public async Task ContainsEntryTest()
        {
            var mm = NewMultimap("mm-ce");
            await mm.Put("k", "v1");
            await mm.Put("k", "v2");

            Assert.True(await mm.ContainsEntry("k", "v1"));
            Assert.False(await mm.ContainsEntry("k", "v999"));
        }

        [Fact]
        public async Task SizeTest()
        {
            var mm = NewMultimap("mm-size");

            Assert.Equal(0, await mm.Size());

            await mm.Put("k1", "v1");
            await mm.Put("k1", "v2");
            await mm.Put("k2", "v3");

            Assert.Equal(3, await mm.Size());
        }
    }
}
