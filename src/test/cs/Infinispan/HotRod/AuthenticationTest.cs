using System;
using NUnit.Framework;
using Infinispan.HotRod.Config;
using System.Collections.Generic;

namespace Infinispan.HotRod.Tests
{
    class AuthenticationTest
    {
        [Test]
        public void PlainAutheticationTest()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222).ConnectionTimeout(90000).SocketTimeout(900);
            //registerServerCAFile(conf, "infinispan-ca.pem");
            AuthenticationStringCallback cbUser = new AuthenticationStringCallback("writer");
            AuthenticationStringCallback cbPass = new AuthenticationStringCallback("somePassword");
            AuthenticationStringCallback cbRealm = new AuthenticationStringCallback("ApplicationRealm");
            IDictionary<int, AuthenticationStringCallback> cbMap = new Dictionary<int, AuthenticationStringCallback>();
            cbMap.Add((int)SaslCallbackId.SASL_CB_USER, cbUser);
            cbMap.Add((int)SaslCallbackId.SASL_CB_PASS, cbPass);
            cbMap.Add((int)SaslCallbackId.SASL_CB_GETREALM, cbRealm);
            conf.Security().Authentication().Enable().SaslMechanism("PLAIN").setupCallback(cbMap);
            conf.Marshaller(new JBasicMarshaller());
            Configuration c = conf.Build();
            RemoteCacheManager remoteManager = new RemoteCacheManager(c, true);
            IRemoteCache<string, string> testCache = remoteManager.GetCache<string, string>("authCache");
            string k1 = "key13";
            string v1 = "boron";
            testCache.Put(k1, v1);
            Assert.AreEqual(v1, testCache.Get(k1));
        }

        [Test]
        public void MD5AutheticationTest()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222).ConnectionTimeout(90000).SocketTimeout(900);
            AuthenticationStringCallback cbUser = new AuthenticationStringCallback("writer");
            AuthenticationStringCallback cbPass = new AuthenticationStringCallback("somePassword");
            AuthenticationStringCallback cbRealm = new AuthenticationStringCallback("ApplicationRealm");
            IDictionary<int, AuthenticationStringCallback> cbMap = new Dictionary<int, AuthenticationStringCallback>();
            cbMap.Add((int)SaslCallbackId.SASL_CB_USER, cbUser);
            cbMap.Add((int)SaslCallbackId.SASL_CB_PASS, cbPass);
            cbMap.Add((int)SaslCallbackId.SASL_CB_GETREALM, cbRealm);
            conf.Security().Authentication().Enable().SaslMechanism("DIGEST-MD5").ServerFQDN("node0").setupCallback(cbMap);
            conf.Marshaller(new JBasicMarshaller());
            Configuration c = conf.Build();
            RemoteCacheManager remoteManager = new RemoteCacheManager(c, true);
            IRemoteCache<string, string> testCache = remoteManager.GetCache<string, string>();
            string k1 = "key13";
            string v1 = "boron";
            testCache.Put(k1, v1);
            Assert.AreEqual(v1, testCache.Get(k1));
        }
        [Test]
        public void PlainAutheticationReadNotWriteTest()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222).ConnectionTimeout(90000).SocketTimeout(900);
            AuthenticationStringCallback cbUser = new AuthenticationStringCallback("reader");
            AuthenticationStringCallback cbPass = new AuthenticationStringCallback("password");
            AuthenticationStringCallback cbRealm = new AuthenticationStringCallback("ApplicationRealm");
            IDictionary<int, AuthenticationStringCallback> cbMap = new Dictionary<int, AuthenticationStringCallback>();
            cbMap.Add((int)SaslCallbackId.SASL_CB_USER, cbUser);
            cbMap.Add((int)SaslCallbackId.SASL_CB_PASS, cbPass);
            cbMap.Add((int)SaslCallbackId.SASL_CB_GETREALM, cbRealm);
            conf.Security().Authentication().Enable().SaslMechanism("PLAIN").setupCallback(cbMap);
            conf.Marshaller(new JBasicMarshaller());
            Configuration c = conf.Build();
            RemoteCacheManager remoteManager = new RemoteCacheManager(c, true);
            IRemoteCache<string, string> testCache = remoteManager.GetCache<string, string>("authCache");
            string k1 = "key13";
            string v1 = "boron";
            string resK1 = testCache.Get(k1);
            try
            {
                testCache.Put(k1, v1);
                Assert.Fail("ERROR: User 'reader' can write!");
            }
            catch (Infinispan.HotRod.Exceptions.HotRodClientException )
            {
            }
        }


    }
}
