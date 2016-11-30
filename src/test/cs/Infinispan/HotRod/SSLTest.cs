using System;
using NUnit.Framework;
using Infinispan.HotRod.Config;

namespace Infinispan.HotRod.Tests
{
    /*
     * Due to https://issues.jboss.org/browse/HRCPP-311 
     * the client registers the trusted server certificate 
     * via MMC (Microsoft Management Console) in Windows 
     * and not via sslConfBuilder.Enable().ServerCAFile(filename).
     * This has to be done manually and prevents running multiple tests at once
     * since all the certifacates have to installed at once and they collide
     * (each test requires a different certificate, not all of them).
     * How to: http://www.databasemart.com/howto/SQLoverssl/How_To_Install_Trusted_Root_Certification_Authority_With_MMC.aspx 
     */
    class SSLTest
    {
        /*
         * Before running this test, install src/test/resoureces/infinispan-ca.pem via MMC.
         */
        [Test]
        public void SSLSuccessfullServerAndClientAuthTest()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222).ConnectionTimeout(90000).SocketTimeout(900);
            registerServerCAFile(conf, "infinispan-ca.pem");
            conf.Marshaller(new JBasicMarshaller());

            RemoteCacheManager remoteManager = new RemoteCacheManager(conf.Build(), true);
            IRemoteCache<string, string> testCache = remoteManager.GetCache<string, string>();

            testCache.Clear();
            string k1 = "key13";
            string v1 = "boron";
            testCache.Put(k1, v1);
            Assert.AreEqual(v1, testCache.Get(k1));
        }

        /*
         * Before running this test, 
         * install src/test/resoureces/keystore_server_sni1.pem via MMC
         * and uninstall all the other certificates.
         */
        [Test]
        [Ignore("https://issues.jboss.org/browse/HRCPP-311")]
        public void SNI1CorrectCredentialsTest() 
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222).ConnectionTimeout(90000).SocketTimeout(900);
            conf.Marshaller(new JBasicMarshaller());
            registerServerCAFile(conf, "keystore_server_sni1.pem", "sni1");
            
            RemoteCacheManager remoteManager = new RemoteCacheManager(conf.Build(), true);
            IRemoteCache<string, string> testCache = remoteManager.GetCache<string, string>();

            testCache.Clear();
            string k1 = "key13";
            string v1 = "boron";
            testCache.Put(k1, v1);
            Assert.AreEqual(v1, testCache.Get(k1));
        }

        /*
         * Before running this test,
         * install src/test/resoureces/keystore_server_sni2.pem via MMC
         * and uninstall all the other certificates.
         */
        [Test]
        [Ignore("https://issues.jboss.org/browse/HRCPP-311")]
        public void SNI2CorrectCredentialsTest()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222).ConnectionTimeout(90000).SocketTimeout(900);
            conf.Marshaller(new JBasicMarshaller());
            registerServerCAFile(conf, "keystore_server_sni2.pem", "sni2");
            RemoteCacheManager remoteManager = new RemoteCacheManager(conf.Build(), true);
            IRemoteCache<string, string> testCache = remoteManager.GetCache<string, string>();

            testCache.Clear();
            string k1 = "key13";
            string v1 = "boron";
            testCache.Put(k1, v1);
            Assert.AreEqual(v1, testCache.Get(k1));
        }

        /*
         * Before running this test, uninstall all certificates via MMC.
         */
        [Test]
        [Ignore("https://issues.jboss.org/browse/HRCPP-311")]
        [ExpectedException(typeof(Infinispan.HotRod.Exceptions.TransportException), ExpectedMessage = "**** Error 0x%x authenticating server credentials!\n")]
        public void SNIUntrustedTest()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222).ConnectionTimeout(90000).SocketTimeout(900);
            conf.Marshaller(new JBasicMarshaller());
            registerServerCAFile(conf, "malicious.pem", "sni3-untrusted");

            RemoteCacheManager remoteManager = new RemoteCacheManager(conf.Build(), true);
            IRemoteCache<string, string> testCache = remoteManager.GetCache<string, string>();

            testCache.Clear();
            string k1 = "key13";
            string v1 = "boron";
            testCache.Put(k1, v1);
            Assert.Fail("Should not get here");
        }

        void registerServerCAFile(ConfigurationBuilder conf, string filename = "", string sni = "")
        {
            SslConfigurationBuilder sslConfB = conf.Ssl();
            if (filename != "")
            {
                checkFileExists(filename);
                sslConfB.Enable().ServerCAFile(filename);
                if (sni != "")
                {
                    sslConfB.SniHostName(sni);
                }
            }
        }

        void checkFileExists(string filename)
        {
            if (!System.IO.File.Exists(filename))
            {
                Console.WriteLine("File not found: " + filename);
                Environment.Exit(-1);
            }
        }
    }
}
