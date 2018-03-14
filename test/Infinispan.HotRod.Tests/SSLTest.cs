using System;
using NUnit.Framework;
using Infinispan.HotRod.Config;

namespace Infinispan.HotRod.Tests.StandaloneHotrodSSLXml
{
    [Category("SecurityTestSuite")]
    [Category("standalone_hotrod_ssl_xml")]
    class SSLTest
    {
        private AuthorizationTester tester = new AuthorizationTester();
        private IRemoteCache<string, string> testCache;
        private IRemoteCache<string, string> scriptCache;
        private IMarshaller marshaller;

        [Test]
        public void WriterSuccessTest()
        {
            ConfigureSecuredCaches("infinispan-ca.pem", "keystore_client.p12");
            tester.TestWriterSuccess(testCache);
        }

        [Test]
        public void WriterPerformsReadsTest()
        {
            ConfigureSecuredCaches("infinispan-ca.pem", "keystore_client.p12");
            tester.TestWriterPerformsReads(testCache);
        }

        [Test]
        public void WriterPerformsSupervisorOpsTest()
        {
            ConfigureSecuredCaches("infinispan-ca.pem", "keystore_client.p12");
            tester.TestWriterPerformsSupervisorOps(testCache, scriptCache, marshaller);
        }

        [Ignore("https://issues.jboss.org/browse/HRCPP-434")]
        [Test]
        public void ClientAuthFailureTest()
        {
            ConfigureSecuredCaches("infinispan-ca.pem", "malicious_client.p12");
            tester.TestWriterSuccess(testCache);
            Assert.Fail("Should not get here");
        }

        [Test]
        public void SNI1CorrectCredentialsTest()
        {
            ConfigureSecuredCaches("keystore_server_sni1_rsa.pem", "keystore_client.p12", "sni1");
            tester.TestWriterSuccess(testCache);
        }

        [Test]
        public void SNI2CorrectCredentialsTest()
        {
            ConfigureSecuredCaches("keystore_server_sni2_rsa.pem", "keystore_client.p12", "sni2");
            tester.TestWriterSuccess(testCache);
        }

        [Test]
        public void SNIUntrustedTest()
        {
            Assert.Throws<Infinispan.HotRod.Exceptions.TransportException>(() => ConfigureSecuredCaches("malicious.pem", "keystore_client.p12", "sni3-untrusted"));
            var ex = Assert.Throws<Infinispan.HotRod.Exceptions.TransportException>(() => tester.TestWriterSuccess(testCache));
            Assert.AreEqual("**** The server certificate did not validate correctly.\n",ex.Message);
        }

        private void ConfigureSecuredCaches(string serverCAFile, string clientCertFile, string sni = "")
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222).ConnectionTimeout(90000).SocketTimeout(900);
            marshaller = new JBasicMarshaller();
            conf.Marshaller(marshaller);
            SslConfigurationBuilder sslConf = conf.Ssl();
            conf.Security().Authentication()
                                .Enable()
                                .SaslMechanism("EXTERNAL")
                                .ServerFQDN("node0");

            RegisterServerCAFile(sslConf, serverCAFile, sni);
            RegisterClientCertificateFile(sslConf, clientCertFile);

            RemoteCacheManager remoteManager = new RemoteCacheManager(conf.Build(), true);

            testCache = remoteManager.GetCache<string, string>();
            scriptCache = remoteManager.GetCache<string, string>("___script_cache");
        }

        private void RegisterServerCAFile(SslConfigurationBuilder conf, string filename = "", string sni = "")
        {
            if (filename != "")
            {
                CheckFileExists(filename);
                conf.Enable().ServerCAFile(filename);
                if (sni != "")
                {
                    conf.SniHostName(sni);
                }
            }
        }

        private void RegisterClientCertificateFile(SslConfigurationBuilder conf, string filename = "")
        {
            if (filename != "")
            {
                CheckFileExists(filename);
                conf.Enable().ClientCertificateFile(filename);
            }
        }

        private void CheckFileExists(string filename)
        {
            Assert.IsTrue(filename!="" && System.IO.File.Exists(filename));
        }
    }   
}
