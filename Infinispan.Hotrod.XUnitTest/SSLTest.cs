using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Infinispan.Hotrod.Tests.Util;
using Xunit;

namespace Infinispan.Hotrod.XUnitTest
{
    public class SSLTestFixture : IAsyncLifetime
    {
        private InfinispanContainer _container;
        public Cache<string, string> cache_verified;
        public Cache<string, string> cache_bad_verified;
        public Cache<string, string> cache;

        public async Task InitializeAsync()
        {
            var confDir = Path.GetFullPath(
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "resources", "conf"));
            var keystorePath = Path.Combine(confDir, "certificates", "keystore.jks");

            _container = new InfinispanContainer("infinispan-ssl.xml",
                resourceMappings: new Dictionary<string, string>
                {
                    [keystorePath] = "/opt/infinispan/server/conf/keystore.jks"
                });
            await _container.StartAsync();

            var host = _container.Host;
            var port = _container.Port;

            var assembly = Assembly.GetExecutingAssembly();

            // Verified TLS: good CA cert
            string fcacert;
            using (var stream = assembly.GetManifestResourceStream("Infinispan.Hotrod.XUnitTest.resources.client.infinispan-ca.pem"))
            using (var reader = new StreamReader(stream))
            {
                fcacert = reader.ReadToEnd();
            }
            var chain = new X509Chain();
            chain.ChainPolicy.CustomTrustStore.Add(X509CertificateLoader.LoadCertificate(StringMarshaller._ASCII.marshall(fcacert)));
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
            var infinispan_verified = new InfinispanClient();
            infinispan_verified.UseTLS = true;
            infinispan_verified.CACert = chain;
            infinispan_verified.AddHost(host, port);
            infinispan_verified.Version = ProtocolVersion.Version31;
            infinispan_verified.ForceReturnValue = false;
            infinispan_verified.ClientIntelligence = ClientIntelligence.Basic;
            cache_verified = infinispan_verified.NewCache(new StringMarshaller(), new StringMarshaller(), "default");

            // Bad verified TLS: wrong CA cert
            string bad_fcacert;
            using (var stream = assembly.GetManifestResourceStream("Infinispan.Hotrod.XUnitTest.resources.client.bad-infinispan-ca.pem"))
            using (var reader = new StreamReader(stream))
            {
                bad_fcacert = reader.ReadToEnd();
            }
            var bad_chain = new X509Chain();
            bad_chain.ChainPolicy.CustomTrustStore.Add(X509CertificateLoader.LoadCertificate(StringMarshaller._ASCII.marshall(bad_fcacert)));
            bad_chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            bad_chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
            var infinispan_bad_verified = new InfinispanClient();
            infinispan_bad_verified.UseTLS = true;
            infinispan_bad_verified.CACert = bad_chain;
            infinispan_bad_verified.AddHost(host, port);
            infinispan_bad_verified.Version = ProtocolVersion.Version31;
            infinispan_bad_verified.ForceReturnValue = false;
            infinispan_bad_verified.ClientIntelligence = ClientIntelligence.Basic;
            cache_bad_verified = infinispan_bad_verified.NewCache(new StringMarshaller(), new StringMarshaller(), "default");

            // Unverified TLS: no CA cert
            var infinispan = new InfinispanClient();
            infinispan.UseTLS = true;
            infinispan.AddHost(host, port);
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
    public class SSLTest : IClassFixture<SSLTestFixture>
    {
        private readonly Cache<string, string> _cache_verified;
        private readonly Cache<string, string> _cache_bad_verified;
        private readonly Cache<string, string> _cache;

        public SSLTest(SSLTestFixture fixture)
        {
            _cache_verified = fixture.cache_verified;
            _cache_bad_verified = fixture.cache_bad_verified;
            _cache = fixture.cache;
        }

        [Fact]
        public async Task SimpleTLSVerifiedGetTest()
        {
            String key = UniqueKey.NextKey();
            Assert.Null(await _cache_verified.Get(key));
            await _cache_verified.Put(key, "carbon");
            Assert.Equal("carbon", await _cache_verified.Get(key));
        }

        [Fact]
        public async Task SimpleTLSGetTest()
        {
            String key = UniqueKey.NextKey();
            Assert.Null(await _cache.Get(key));
            await _cache.Put(key, "carbon");
            Assert.Equal("carbon", await _cache.Get(key));
        }

        [Fact]
        public async Task SimpleTLSBadVerifiedGetTest()
        {
            String key = UniqueKey.NextKey();
            await Assert.ThrowsAsync<InfinispanException>(() => _cache_bad_verified.Get(key));
        }
    }
}
