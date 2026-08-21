using System.Linq;
using System.Threading.Tasks;
using Infinispan.Hotrod;
using Infinispan.Hotrod.Tests.Util;
using Xunit;

namespace Infinispan.Hotrod.XUnitTest
{
    public class CacheAdminTestFixture : IAsyncLifetime
    {
        private InfinispanContainer _container;
        public InfinispanClient infinispan = new InfinispanClient();

        public async Task InitializeAsync()
        {
            _container = new InfinispanContainer("infinispan-noauth.xml");
            await _container.StartAsync();
            infinispan.AddHost(_container.Host, _container.Port);
            infinispan.Version = 0x1f;
            infinispan.ClientIntelligence = 0x01;
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }
    }

    [Collection("MainSequence")]
    public class CacheAdminTest : IClassFixture<CacheAdminTestFixture>
    {
        private readonly InfinispanClient _client;

        public CacheAdminTest(CacheAdminTestFixture fixture)
        {
            _client = fixture.infinispan;
        }

        [Fact]
        public async Task GetCacheNamesTest()
        {
            var admin = _client.Administration();
            var names = await admin.GetCacheNames();
            Assert.Contains("default", names);
        }

        [Fact]
        public async Task CreateAndRemoveCacheTest()
        {
            var admin = _client.Administration();
            await admin.CreateCache("admin-test-cache");

            var names = await admin.GetCacheNames();
            Assert.Contains("admin-test-cache", names);

            await admin.RemoveCache("admin-test-cache");

            names = await admin.GetCacheNames();
            Assert.DoesNotContain("admin-test-cache", names);
        }

        [Fact]
        public async Task GetOrCreateCacheTest()
        {
            var admin = _client.Administration();
            await admin.GetOrCreateCache("admin-goc-cache");
            await admin.GetOrCreateCache("admin-goc-cache");

            var names = await admin.GetCacheNames();
            Assert.Contains("admin-goc-cache", names);

            await admin.RemoveCache("admin-goc-cache");
        }

        [Fact]
        public async Task CreateCacheWithConfigurationTest()
        {
            var admin = _client.Administration();
            var config = "<local-cache/>";
            await admin.CreateCache("admin-config-cache", config);

            var names = await admin.GetCacheNames();
            Assert.Contains("admin-config-cache", names);

            await admin.RemoveCache("admin-config-cache");
        }

        [Fact]
        public async Task CreateTemplateAndCacheTest()
        {
            var admin = _client.Administration();
            await admin.CreateTemplate("my-template", "<local-cache/>");
            await admin.CreateCacheWithTemplate("admin-tpl-cache", "my-template");

            var names = await admin.GetCacheNames();
            Assert.Contains("admin-tpl-cache", names);

            await admin.RemoveCache("admin-tpl-cache");
            await admin.RemoveTemplate("my-template");
        }

        [Fact]
        public async Task SchemaCreateAndDeleteTest()
        {
            var admin = _client.Administration();
            var schemas = admin.Schemas();

            var proto = "package admin_test;\nmessage TestEntity {\n  required string name = 1;\n}\n";
            await schemas.Create("admin_test.proto", proto);

            await schemas.Delete("admin_test.proto");
        }
    }
}
