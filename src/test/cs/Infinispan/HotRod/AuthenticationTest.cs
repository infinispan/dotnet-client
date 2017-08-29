using System;
using NUnit.Framework;
using Infinispan.HotRod.Config;
using System.Collections.Generic;

namespace Infinispan.HotRod.Tests
{
    class AuthenticationTest
    {

        [Test]
        public void PlainAutheticationTestWithEasySaslSetup()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222).ConnectionTimeout(90000).SocketTimeout(900);
            conf.Security().Authentication()
                                .Enable()
                                .ServerFQDN("node0")
                                .SaslMechanism("PLAIN")
                                .SetupStringCallback("supervisor", "lessStrongPassword", "ApplicationRealm");
            conf.Marshaller(new JBasicMarshaller());
            Configuration c = conf.Build();
            RemoteCacheManager remoteManager = new RemoteCacheManager(c, true);
            IRemoteCache<string, string> testCache = remoteManager.GetCache<string, string>("authCache");
            TestPut(testCache);
        }

        [Test]
        public void MD5AutheticationTestWithEasySaslSetup()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222).ConnectionTimeout(90000).SocketTimeout(900);
            conf.Security().Authentication()
                                .Enable()
                                .ServerFQDN("node0")
                                .SaslMechanism("DIGEST-MD5")
                                .SetupStringCallback("supervisor", "lessStrongPassword", "ApplicationRealm");
            conf.Marshaller(new JBasicMarshaller());
            Configuration c = conf.Build();
            RemoteCacheManager remoteManager = new RemoteCacheManager(c, true);
            IRemoteCache<string, string> testCache = remoteManager.GetCache<string, string>("authCache");
            TestPut(testCache);
        }

        [Test]
        public void PlainAutheticationTest()
        {
            IRemoteCache<string, string> testCache = InitCache("PLAIN", "node0");
            TestPut(testCache);
        }

        [Test]
        public void MD5AutheticationTest()
        {
            IRemoteCache<string, string> testCache = InitCache("DIGEST-MD5", "node0");
            TestPut(testCache);
        }


        [Test]
        [ExpectedException(typeof(Infinispan.HotRod.Exceptions.HotRodClientException))]
        [Ignore("https://issues.jboss.org/browse/HRCPP-385")]
        public void WrongServerNamePlainAuthTest()
        {
            IRemoteCache<string, string> testCache = InitCache("PLAIN", "nonExistentNode");
            TestPut(testCache);
        }

        [Test]
        [ExpectedException(typeof(Infinispan.HotRod.Exceptions.HotRodClientException))]
        public void WrongServerNameDigestAuthTest()
        {
            IRemoteCache<string, string> testCache = InitCache("DIGEST-MD5", "nonExistentNode");
            TestPut(testCache);
        }

        private void TestPut(IRemoteCache<string, string> testCache)
        {
            string k1 = "key13";
            string v1 = "boron";
            testCache.Put(k1, v1);
            Assert.AreEqual(v1, testCache.Get(k1));
        }

        private IRemoteCache<string, string> InitCache(string mech, string serverName)
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222).ConnectionTimeout(90000).SocketTimeout(900);
            //registerServerCAFile(conf, "infinispan-ca.pem");
            AuthenticationStringCallback cbUser = new AuthenticationStringCallback("supervisor");
            AuthenticationStringCallback cbPass = new AuthenticationStringCallback("lessStrongPassword");
            AuthenticationStringCallback cbRealm = new AuthenticationStringCallback("ApplicationRealm");
            IDictionary<int, AuthenticationStringCallback> cbMap = new Dictionary<int, AuthenticationStringCallback>();
            cbMap.Add((int)SaslCallbackId.SASL_CB_USER, cbUser);
            cbMap.Add((int)SaslCallbackId.SASL_CB_PASS, cbPass);
            cbMap.Add((int)SaslCallbackId.SASL_CB_GETREALM, cbRealm);
            conf.Security().Authentication()
                                .Enable()
                                .ServerFQDN(serverName)
                                .SaslMechanism(mech)
                                .SetupCallback(cbMap);
            conf.Marshaller(new JBasicMarshaller());
            Configuration c = conf.Build();
            RemoteCacheManager remoteManager = new RemoteCacheManager(c, true);
            return remoteManager.GetCache<string, string>("authCache");
        }
    }
}
