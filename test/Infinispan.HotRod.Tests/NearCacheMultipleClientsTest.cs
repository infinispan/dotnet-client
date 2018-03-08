using Infinispan.HotRod.Config;
using NUnit.Framework;
using Infinispan.HotRod.Tests.Util;

namespace Infinispan.HotRod.Tests.StandaloneXml
{
    [TestFixture]
    [Category("standalone_xml")]
    [Category("DefaultTestSuite")]
    public class NearCacheMultipleClientsTest
    {
        RemoteCacheManager remoteManager1;
        RemoteCacheManager remoteManager2;
        IMarshaller marshaller;

        [OneTimeSetUp]
        public void BeforeClass()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222)
            .ConnectionTimeout(90000).SocketTimeout(6000)
            .NearCache().Mode(NearCacheMode.INVALIDATED).MaxEntries(-1);
            marshaller = new JBasicMarshaller();
            conf.Marshaller(marshaller);
            remoteManager1 = new RemoteCacheManager(conf.Build(), true);
            remoteManager2 = new RemoteCacheManager(conf.Build(), true);
        }

        [OneTimeTearDown]
        public void AfterClass()
        {
            remoteManager1.Stop();
            remoteManager2.Stop();
        }

        [Test]
        public void ClientsInvalidatedTest()
        {
            var cache1 = remoteManager1.GetCache<string, string>();
            var cache2 = remoteManager2.GetCache<string, string>();
            cache1.Clear();
            cache2.Clear();
            var k = "k";
            Assert.IsNull(cache1.Get(k));
            Assert.IsNull(cache2.Get(k));
            cache1.Put(k, "v1");
            var stats1 = cache2.Stats();
            //Get needs to go remotely because this is first time cache2 client reads the value
            Assert.AreEqual("v1", cache2.Get(k));
            //Get is only local now
            Assert.AreEqual("v1", cache2.Get(k));
            var stats2 = cache2.Stats();
            Assert.AreEqual(stats1.GetIntStatistic("hits") + 1, stats2.GetIntStatistic("hits"), "Client 2 did not reach the server!");
            cache1.Put(k, "v2");
            //Get needs to go remotely
            TimeUtils.WaitFor(() => cache2.Get(k).Equals("v2"));
            var stats3 = cache2.Stats();
            Assert.AreEqual(stats2.GetIntStatistic("hits") + 1, stats3.GetIntStatistic("hits"), "Client 2 did not reach the server for new value!");
            var cache1Stats1 = cache1.Stats();
            cache2.Put(k, "v3");
            //Get needs to go remotely
            TimeUtils.WaitFor(() => cache1.Get(k).Equals("v3"));
            var cache1Stats2 = cache1.Stats();
            Assert.AreEqual(cache1Stats1.GetIntStatistic("hits") + 1, cache1Stats2.GetIntStatistic("hits"), "Client 1 did not reach the server for new value!");
        }
    }
}
