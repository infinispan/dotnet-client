using System;
using NUnit.Framework;
using Infinispan.HotRod.Config;
using System.Runtime.InteropServices;

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
        private RemoteCacheManager remoteManager;

        [TearDown]
        public void stopRemoteManager()
        {
            if ( remoteManager !=null ) {
                remoteManager.Stop();
            }
        }

        [Ignore("ALPN setup on Windows doesn't work")]
        [Test]
        public void WriterSuccessTest()
        {
            string clientCertName;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                clientCertName = "keystore_client.p12";
            }  else  if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                clientCertName = "truststore_client.pem";
            }  else { Assert.Fail(); return; }
            ConfigureSecuredCaches("infinispan-ca.pem", clientCertName);
            tester.TestWriterSuccess(testCache);
        }

        [Ignore("ALPN setup on Windows doesn't work")]
        [Test]
        public void WriterPerformsReadsTest()
        {
            string clientCertName;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                clientCertName = "keystore_client.p12";
            }  else  if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                clientCertName = "truststore_client.pem";
            }  else { Assert.Fail(); return; }
            ConfigureSecuredCaches("infinispan-ca.pem", clientCertName);
            tester.TestWriterPerformsReads(testCache);
        }

        [Ignore("ALPN setup on Windows doesn't work")]
        [Test]
        public void WriterPerformsSupervisorOpsTest()
        {
            string clientCertName;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                clientCertName = "keystore_client.p12";
            }  else  if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                clientCertName = "truststore_client.pem";
            }  else { Assert.Fail(); return; }
            ConfigureSecuredCaches("infinispan-ca.pem", clientCertName);
            tester.TestWriterPerformsSupervisorOps(testCache, scriptCache, marshaller);
        }

        [Ignore("https://issues.jboss.org/browse/HRCPP-434")]
        [Test]
        public void ClientAuthFailureTest()
        {
            string clientCertName;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                clientCertName = "keystore_client.p12";
            }  else  if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                clientCertName = "truststore_client.pem";
            }  else { Assert.Fail(); return; }
            ConfigureSecuredCaches("infinispan-ca.pem", clientCertName);
            tester.TestWriterSuccess(testCache);
            Assert.Fail("Should not get here");
        }

        [Ignore("ALPN setup on Windows doesn't work")]
        [Test]
        public void SNI1CorrectCredentialsTest()
        {
            string clientCertName;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                clientCertName = "keystore_client.p12";
            }  else  if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                clientCertName = "truststore_client.pem";
            }  else { Assert.Fail(); return; }
            ConfigureSecuredCaches("keystore_server_sni1_rsa.pem", clientCertName, "sni1");
            tester.TestWriterSuccess(testCache);
        }

        [Ignore("ALPN setup on Windows doesn't work")]
        [Test]
        public void SNI2CorrectCredentialsTest()
        {
            string clientCertName;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                clientCertName = "keystore_client.p12";
            }  else  if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                clientCertName = "truststore_client.pem";
            }  else { Assert.Fail(); return; }
            ConfigureSecuredCaches("keystore_server_sni2_rsa.pem", clientCertName, "sni2");
            tester.TestWriterSuccess(testCache);
        }

        [Ignore("ALPN setup on Windows doesn't work")]
        [Test]
        public void SNIUntrustedTest()
        {
            string clientCertName;
            string errMessage;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                clientCertName = "keystore_client.p12";
                errMessage = "**** The server certificate did not validate correctly.\n";
            }  else  if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                clientCertName = "truststore_client.pem";
                errMessage = "SSL_get_peer_certificate";
            }  else { Assert.Fail(); return; }
            var ex = Assert.Throws<Infinispan.HotRod.Exceptions.TransportException>(() => ConfigureSecuredCaches("malicious.pem", clientCertName, "sni3-untrusted"));
            Assert.AreEqual(errMessage, ex.Message);
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

            remoteManager = new RemoteCacheManager(conf.Build(), true);

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
