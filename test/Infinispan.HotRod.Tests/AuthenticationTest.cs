using NUnit.Framework;
using Infinispan.HotRod.Config;

namespace Infinispan.HotRod.Tests.ClusteredSaslCsXml2
{
    [TestFixture]
    [Category("clustered_sasl_cs_xml_2")]
    public class AuthenticationTest
    {
        private const string USER = "supervisor";
        private const string PASS = "lessStrongPassword";

        [Test]
        public void PlainAutheticationWithEasySaslSetupTest()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222).ConnectionTimeout(90000).SocketTimeout(900);
            conf.Security().Authentication()
                                .Enable()
                                .ServerFQDN("node0")
                                .SaslMechanism("PLAIN")
                                .SetupCallback("supervisor", "lessStrongPassword", "ApplicationRealm");
            conf.Marshaller(new JBasicMarshaller());
            Configuration c = conf.Build();
            RemoteCacheManager remoteManager = new RemoteCacheManager(c, true);
            IRemoteCache<string, string> testCache = remoteManager.GetCache<string, string>("authCache");
            TestPut(testCache);
        }

        [Test]
        public void MD5AutheticationWithEasySaslSetupTest()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222).ConnectionTimeout(90000).SocketTimeout(900);
            conf.Security().Authentication()
                                .Enable()
                                .ServerFQDN("node0")
                                .SaslMechanism("DIGEST-MD5")
                                .SetupCallback("supervisor", "lessStrongPassword", "ApplicationRealm");
            conf.Marshaller(new JBasicMarshaller());
            Configuration c = conf.Build();
            RemoteCacheManager remoteManager = new RemoteCacheManager(c, true);
            IRemoteCache<string, string> testCache = remoteManager.GetCache<string, string>("authCache");
            TestPut(testCache);
        }

        [Test]
        public void PlainAutheticationTest()
        {
            IRemoteCache<string, string> testCache = InitCache("PLAIN", "node0", USER, PASS);
            TestPut(testCache);
        }

        [Test]
        public void MD5AutheticationTest()
        {
            IRemoteCache<string, string> testCache = InitCache("DIGEST-MD5", "node0", USER, PASS);
            TestPut(testCache);
        }

        [Test]
        public void PlainAutheticationWrongPasswordTest()
        {
            IRemoteCache<string, string> testCache = InitCache("PLAIN", "node0", USER, "mallicious_password");
            Assert.Throws<Infinispan.HotRod.Exceptions.HotRodClientException>(() => TestPut(testCache));
        }

        [Test]
        public void DigestAutheticationWrongPasswordTest()
        {
            IRemoteCache<string, string> testCache = InitCache("DIGEST-MD5", "node0", USER, "mallicious_password");
            Assert.Throws<Infinispan.HotRod.Exceptions.HotRodClientException>(() => TestPut(testCache));
        }

        [Test]
        public void WrongServerNameDigestAuthTest()
        {
            IRemoteCache<string, string> testCache = InitCache("DIGEST-MD5", "nonExistentNode", USER, PASS);
            Assert.Throws<Infinispan.HotRod.Exceptions.HotRodClientException>(() => TestPut(testCache));
        }

        private void TestPut(IRemoteCache<string, string> testCache)
        {
            string k1 = "key13";
            string v1 = "boron";
            testCache.Put(k1, v1);
            Assert.AreEqual(v1, testCache.Get(k1));
        }

        private IRemoteCache<string, string> InitCache(string mech, string serverName, string username, string password)
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222).ConnectionTimeout(90000).SocketTimeout(900);
            //registerServerCAFile(conf, "infinispan-ca.pem");
            conf.Security().Authentication()
                                .Enable()
                                .ServerFQDN(serverName)
                                .SaslMechanism(mech)
                                .SetupCallback(() => username, () => password, () => "ApplicationRealm");
            conf.Marshaller(new JBasicMarshaller());
            Configuration c = conf.Build();
            RemoteCacheManager remoteManager = new RemoteCacheManager(c, true);
            return remoteManager.GetCache<string, string>("authCache");
        }
    }
}
