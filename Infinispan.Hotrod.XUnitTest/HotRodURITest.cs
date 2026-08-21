using System;
using System.Threading.Tasks;
using Infinispan.Hotrod;
using Infinispan.Hotrod.Tests.Util;
using Xunit;

namespace Infinispan.Hotrod.XUnitTest
{
    public class HotRodURITest
    {
        [Fact]
        public void ParseBasicUri()
        {
            var client = InfinispanClient.FromUri("hotrod://localhost");
            Assert.NotNull(client);
            Assert.False(client.UseTLS);
            Assert.Null(client.User);
            Assert.Null(client.AuthMech);
        }

        [Fact]
        public void ParseUriWithPort()
        {
            var client = InfinispanClient.FromUri("hotrod://myhost:9999");
            Assert.NotNull(client);
        }

        [Fact]
        public void ParseUriWithCredentials()
        {
            var client = InfinispanClient.FromUri("hotrod://admin:secret@localhost:11222");
            Assert.Equal("admin", client.User);
            Assert.Equal("secret", client.Password);
            Assert.Equal("SCRAM-SHA-256", client.AuthMech);
        }

        [Fact]
        public void ParseTlsUri()
        {
            var client = InfinispanClient.FromUri("hotrods://secure.example.com");
            Assert.True(client.UseTLS);
        }

        [Fact]
        public void ParseTlsWithCredentials()
        {
            var client = InfinispanClient.FromUri("hotrods://user:pass@host1:11222");
            Assert.True(client.UseTLS);
            Assert.Equal("user", client.User);
            Assert.Equal("pass", client.Password);
        }

        [Fact]
        public void ParseUriWithSaslMechanism()
        {
            var client = InfinispanClient.FromUri("hotrod://admin:pass@localhost?sasl_mechanism=PLAIN");
            Assert.Equal("PLAIN", client.AuthMech);
        }

        [Fact]
        public void ParseUriDefaultPort()
        {
            var client = InfinispanClient.FromUri("hotrod://myhost");
            Assert.NotNull(client);
        }

        [Fact]
        public void ParseUriMultipleHosts()
        {
            var client = InfinispanClient.FromUri("hotrod://host1:11222,host2:11223,host3");
            Assert.NotNull(client);
        }

        [Fact]
        public void ParseUriIPv6()
        {
            var client = InfinispanClient.FromUri("hotrod://[::1]:11222");
            Assert.NotNull(client);
        }

        [Fact]
        public void ParseUriWithMultipleProperties()
        {
            var client = InfinispanClient.FromUri(
                "hotrod://admin:pass@localhost:11222?sasl_mechanism=SCRAM-SHA-256&connect_timeout=5000&socket_timeout=2000");
            Assert.Equal("admin", client.User);
            Assert.Equal("SCRAM-SHA-256", client.AuthMech);
        }

        [Fact]
        public void ParseUriWithToken()
        {
            var client = InfinispanClient.FromUri("hotrod://localhost?token=my-bearer-token");
            Assert.Equal("OAUTHBEARER", client.AuthMech);
        }

        [Fact]
        public void ParseUriInvalidScheme()
        {
            Assert.Throws<ArgumentException>(() =>
                InfinispanClient.FromUri("http://localhost"));
        }

        [Fact]
        public void ParseUriEmptyHost()
        {
            Assert.Throws<ArgumentException>(() =>
                InfinispanClient.FromUri("hotrod://"));
        }

        [Fact]
        public void ParseUriUnknownProperty()
        {
            Assert.Throws<ArgumentException>(() =>
                InfinispanClient.FromUri("hotrod://localhost?bogus=1"));
        }

        [Fact]
        public void ParseUriTlsProperties()
        {
            var client = InfinispanClient.FromUri(
                "hotrods://localhost:11222?sni_host_name=infinispan.example.com&ssl_hostname_validation=false");
            Assert.True(client.UseTLS);
        }

        [Fact]
        public void ParseUriTlsPropertyAliases()
        {
            var client = InfinispanClient.FromUri(
                "hotrods://localhost:11222?sni_host=infinispan.example.com&verify_hostname=true");
            Assert.True(client.UseTLS);
        }

        [Fact]
        public void ParseUriClientIntelligence()
        {
            var client = InfinispanClient.FromUri("hotrod://localhost?client_intelligence=basic");
            Assert.Equal(ClientIntelligence.Basic, client.ClientIntelligence);

            client = InfinispanClient.FromUri("hotrod://localhost?client_intelligence=hash_distribution_aware");
            Assert.Equal(ClientIntelligence.HashDistributionAware, client.ClientIntelligence);

            client = InfinispanClient.FromUri("hotrod://localhost?client_intelligence=TopologyAware");
            Assert.Equal(ClientIntelligence.TopologyAware, client.ClientIntelligence);
        }

        [Fact]
        public void ParseUriProtocolVersion()
        {
            var client = InfinispanClient.FromUri("hotrod://localhost?protocol_version=version31");
            Assert.Equal(ProtocolVersion.Version31, client.Version);

            client = InfinispanClient.FromUri("hotrod://localhost?version=version40");
            Assert.Equal(ProtocolVersion.Version40, client.Version);
        }

        [Fact]
        public void ParseUriInvalidClientIntelligence()
        {
            Assert.Throws<ArgumentException>(() =>
                InfinispanClient.FromUri("hotrod://localhost?client_intelligence=bogus"));
        }

        [Fact]
        public async Task ConnectWithUri()
        {
            var container = new InfinispanContainer("infinispan-noauth.xml");
            await container.StartAsync();
            try
            {
                var client = InfinispanClient.FromUri($"hotrod://{container.Host}:{container.Port}");
                client.Version = ProtocolVersion.Version31;
                client.ClientIntelligence = ClientIntelligence.Basic;

                var cache = client.NewCache(new StringMarshaller(), new StringMarshaller(), "default");
                await cache.Put("uri-test", "hello");
                var result = await cache.Get("uri-test");
                Assert.Equal("hello", result);
            }
            finally
            {
                await container.DisposeAsync();
            }
        }

        [Fact]
        public async Task ConnectWithUriAuth()
        {
            var container = new InfinispanContainer("infinispan-noauth.xml", "admin", "password");
            await container.StartAsync();
            try
            {
                var client = InfinispanClient.FromUri(
                    $"hotrod://admin:password@{container.Host}:{container.Port}?sasl_mechanism=SCRAM-SHA-256");
                client.Version = ProtocolVersion.Version31;
                client.ClientIntelligence = ClientIntelligence.Basic;

                var cache = client.NewCache(new StringMarshaller(), new StringMarshaller(), "default");
                await cache.Put("uri-auth-test", "world");
                var result = await cache.Get("uri-auth-test");
                Assert.Equal("world", result);
            }
            finally
            {
                await container.DisposeAsync();
            }
        }
    }
}
