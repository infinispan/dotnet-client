using Infinispan.HotRod.Config;
using Infinispan.HotRod.Tests.Util;
using Infinispan.HotRod.Transport;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Infinispan.HotRod.Tests.ClusteredXml2
{
    [TestFixture]
    [Category("clustered_xml_2")]
    [Category("AdminOpTestSuite")]
    public class RemoteCacheAdminTest
    {
        RemoteCacheManager remoteManager;
        IMarshaller marshaller;

        [SetUp]
        public void BeforeAnyTest()
        {
            ClusterTestSuite.EnsureServersUp();
            remoteManager = ClusterTestSuite.getRemoteCacheManager();
        }

        [TearDown]
        public void AfterAnyTest()
        {
            remoteManager.Stop();
        }

        [Test]
        public void alreadyExistingCacheTest()
        {
            remoteManager.Administration().CreateCache<string, string>("alreadyExistingCache", "template");
            var ex = Assert.Throws<Infinispan.HotRod.Exceptions.HotRodClientException>(() =>
                { remoteManager.Administration().CreateCache<string, string>("alreadyExistingCache", "template"); });
            StringAssert.Contains("ISPN000507:", ex.Message);
        }

        [Test]
        public void nonExistentTemplateTest()
        {
            var ex = Assert.Throws<Infinispan.HotRod.Exceptions.HotRodClientException>(() =>
            { remoteManager.Administration().CreateCache<string, string>("cache4AdminTest", "nonExistentTemplate"); });
            StringAssert.Contains("ISPN000374:", ex.Message);
        }

        [Test]
        public void getOrCreateWithoutTemplateTest()
        {
            var cache = remoteManager.Administration().GetOrCreateCache<string,string>("default", (String)null);
            Assert.NotNull(cache);
        }

        [Test]
        public void cacheCreateWithXMLConfigurationAndGetCacheTest()
        {
            String cacheName = "cache4XmlCreateAndGetTest";
            remoteManager.Administration().CreateCacheWithXml<object, object>(cacheName,
               "<infinispan><cache-container><distributed-cache name=\""+cacheName+"\"/></cache-container></infinispan>");
            var cache = remoteManager.GetCache<object, object>(cacheName);
            Assert.NotNull(cache);
        }

        [Test]
        [Ignore("HRCPP-468 Permanent doesn't work in windows")]
        public void permanentCacheTest()
        {
            String cacheName = "cache4PermanentTest";
            var flags = new HashSet<AdminFlag>();
            flags.Add(AdminFlag.PERMANENT); 
            remoteManager.Administration().WithFlags(flags).CreateCache<string,string>(cacheName, "template");
            remoteManager.Stop();
            ClusterTestSuite.server1.ShutDownHotrodServer();
            ClusterTestSuite.server2.ShutDownHotrodServer();
            ClusterTestSuite.server1.StartHotRodServer();
            ClusterTestSuite.server2.StartHotRodServer();
            remoteManager = ClusterTestSuite.getRemoteCacheManager();
            var ex = Assert.Throws<Infinispan.HotRod.Exceptions.HotRodClientException>(() =>
            { remoteManager.Administration().CreateCache<string, string>(cacheName, "template"); });
            StringAssert.Contains("ISPN000507:", ex.Message);
        }
    }
}
