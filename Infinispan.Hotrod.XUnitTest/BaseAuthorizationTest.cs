using System;
using System.Threading.Tasks;
using Infinispan.Hotrod.Tests.Util;
using Xunit;

namespace Infinispan.Hotrod.XUnitTest
{
    public class AuthorizationCacheTestFixture : IAsyncLifetime
    {
        private InfinispanContainer _container;
        public string Host { get; private set; }
        public int Port { get; private set; }

        public async Task InitializeAsync()
        {
            _container = new InfinispanContainer("infinispan-sasl.xml", "admin", "strongPassword");
            await _container.StartAsync();
            await _container.CreateUserAsync("supervisor", "lessStrongPassword", "supervisor");
            await _container.CreateUserAsync("reader", "password", "reader");
            await _container.CreateUserAsync("writer", "somePassword", "writer");
            await _container.CreateUserAsync("executor", "executorPassword", "executor");
            Host = _container.Host;
            Port = _container.Port;
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }
    }

    [Collection("MainSequence")]
    public abstract class BaseAuthorizationTest : IClassFixture<AuthorizationCacheTestFixture>
    {
        private readonly AuthorizationCacheTestFixture _fixture;

        public BaseAuthorizationTest(AuthorizationCacheTestFixture fixture)
        {
            _fixture = fixture;
            BeforeClass();
        }

        protected AuthorizationTester tester = new AuthorizationTester();

        public abstract string GetMech();
        public Cache<String, String> readerCache;
        public Cache<String, String> writerCache;
        public Cache<String, String> supervisorCache;
        public Cache<String, String> adminCache;
        public Cache<String, String> scriptCache;
        public const string PROTOBUF_SCRIPT_CACHE_NAME = "___script_cache";
        public const string AUTH_CACHE = "authCache";

        private Cache<String, String> InitCache(string user, string password, string cacheName = AUTH_CACHE)
        {
            var ispnCluster = new InfinispanClient();
            ispnCluster.User = user;
            ispnCluster.Password = password;
            ispnCluster.AuthMech = GetMech();
            ispnCluster.Domain = "node0";
            ispnCluster.Version = ProtocolVersion.Version31;
            ispnCluster.ClientIntelligence = ClientIntelligence.Basic;
            ispnCluster.ForceReturnValue = false;
            ispnCluster.AddHost(_fixture.Host, _fixture.Port);
            return ispnCluster.NewCache(new StringMarshaller(), new StringMarshaller(), cacheName);
        }

        private void BeforeClass()
        {
            readerCache = InitCache("reader", "password");
            writerCache = InitCache("writer", "somePassword");
            supervisorCache = InitCache("supervisor", "lessStrongPassword");
            adminCache = InitCache("admin", "strongPassword");
            scriptCache = InitCache("admin", "strongPassword", PROTOBUF_SCRIPT_CACHE_NAME);
        }
    }
}
