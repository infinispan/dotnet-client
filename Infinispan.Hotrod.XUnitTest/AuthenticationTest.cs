using System;
using System.Threading.Tasks;
using Infinispan.Hotrod.Tests.Util;
using Xunit;

namespace Infinispan.Hotrod.XUnitTest
{
    public class AuthenticationTestFixture : IAsyncLifetime
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
            Host = _container.Host;
            Port = _container.Port;
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }
    }

    [Collection("MainSequence")]
    public class AuthenticationTest : IClassFixture<AuthenticationTestFixture>
    {
        private readonly string _host;
        private readonly int _port;
        private const string USER = "supervisor";
        private const string PASS = "lessStrongPassword";
        private const string AUTH_CACHE = "authCache";

        public AuthenticationTest(AuthenticationTestFixture fixture)
        {
            _host = fixture.Host;
            _port = fixture.Port;
        }

        [Fact]
        public async Task PlainAuthenticationTest()
        {
            Cache<string, string> testCache = InitCache("PLAIN", USER, PASS);
            await TestPut(testCache);
        }

        [Fact]
        public async Task ScramSha256AuthenticationTest()
        {
            Cache<string, string> testCache = InitCache("SCRAM-SHA-256", USER, PASS);
            await TestPut(testCache);
        }

        [Fact]
        public async Task PlainAuthenticationWithEasySaslSetupTest()
        {
            var ispnCluster = new InfinispanClient();
            ispnCluster.User = USER;
            ispnCluster.Password = PASS;
            ispnCluster.AuthMech = "PLAIN";
            ispnCluster.Domain = "node0";
            ispnCluster.Version = 0x1f;
            ispnCluster.ClientIntelligence = 0x01;
            ispnCluster.ForceReturnValue = false;
            ispnCluster.AddHost(_host, _port);
            var cache = ispnCluster.NewCache(new StringMarshaller(), new StringMarshaller(), AUTH_CACHE);
            await TestPut(cache);
        }

        [Fact]
        public async Task ScramSha256AuthenticationWithEasySaslSetupTest()
        {
            var ispnCluster = new InfinispanClient();
            ispnCluster.User = USER;
            ispnCluster.Password = PASS;
            ispnCluster.AuthMech = "SCRAM-SHA-256";
            ispnCluster.Domain = "node0";
            ispnCluster.Version = 0x1f;
            ispnCluster.ClientIntelligence = 0x01;
            ispnCluster.ForceReturnValue = false;
            ispnCluster.AddHost(_host, _port);
            var cache = ispnCluster.NewCache(new StringMarshaller(), new StringMarshaller(), AUTH_CACHE);
            await TestPut(cache);
        }

        private async Task TestPut(Cache<string, string> testCache)
        {
            string k1 = UniqueKey.NextKey();
            string v1 = "boron";
            await testCache.Put(k1, v1);
            Assert.Equal(v1, await testCache.Get(k1));
        }

        private Cache<string, string> InitCache(string mech, string user, string password, string cacheName = AUTH_CACHE)
        {
            var ispnCluster = new InfinispanClient();
            ispnCluster.User = user;
            ispnCluster.Password = password;
            ispnCluster.AuthMech = mech;
            ispnCluster.Domain = "node0";
            ispnCluster.Version = 0x1f;
            ispnCluster.ClientIntelligence = 0x01;
            ispnCluster.ForceReturnValue = false;
            ispnCluster.AddHost(_host, _port);
            return ispnCluster.NewCache(new StringMarshaller(), new StringMarshaller(), cacheName);
        }
    }
}
