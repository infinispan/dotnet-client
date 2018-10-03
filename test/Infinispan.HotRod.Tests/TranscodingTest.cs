using Infinispan.HotRod;
using Infinispan.HotRod.Config;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Infinispan.HotRod.Tests.Util;
using System.Threading.Tasks;
using System.Threading;
using System.Text;

namespace Infinispan.HotRod.Tests.StandaloneXml
{
    class TransparentMarshaller : IMarshaller
    {
        public bool IsMarshallable(object o)
        {
            return o is string;
        }

        public object ObjectFromByteBuffer(byte[] buf)
        {

            return Encoding.UTF8.GetString(buf);
        }

        public object ObjectFromByteBuffer(byte[] buf, int offset, int length)
        {
            return ObjectFromByteBuffer(buf);
        }

        public byte[] ObjectToByteBuffer(object obj)
        {
            return Encoding.UTF8.GetBytes((string)obj);
        }

        public byte[] ObjectToByteBuffer(object obj, int estimatedSize)
        {
            return this.ObjectToByteBuffer(obj);
        }
    }
    [TestFixture]
    [Category("standalone_xml")]
    [Category("DefaultTestSuite")]
    public class TranscodingTest
    {
        private IRemoteCache<object, object> cache;
        private IRemoteCache<object, object> transcodingCache;

        [OneTimeSetUp]
        public void BeforeClass()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222);
            conf.ConnectionTimeout(90000).SocketTimeout(6000);
            conf.ProtocolVersion("2.8");
            conf.Marshaller(new JBasicMarshaller());
            RemoteCacheManager remoteManager = new RemoteCacheManager(conf.Build(), true);
            cache = remoteManager.GetCache<object, object>();
            transcodingCache = remoteManager.GetCache<object, object>("transcodingCache");

        }

        [Test]
        public void RawCacheDefaultJbossMediaTypeTest()
        {
            cache.Put("k1", "value1");
            Assert.AreEqual("value1", cache.Get("k1"));
        }

        // Server uses raw bytes as default cache type, so tests are
        // done with a transparent marshaller and with assertion on raw byte result
        // this means that if an entry is store using jboss format the jboss marshalling
        // preamble needs to be check in the assertions.

        [Test]
        public void RawCachePutJbossGetJsonMediaTypeTest()
        {
            var df = new DataFormat();
            df.Marshaller = new TransparentMarshaller();
            df.KeyMediaType = "application/json";
            df.ValueMediaType = "application/json";
            var jsonCache = cache.WithDataFormat(df);
            cache.Put("k1", "value1");
            var retVal = jsonCache.Get("\x0003>\x0002k1");
            Assert.AreEqual("\"\\u0003>\\u0006value1\"", retVal);
        }

        [Test]
        public void RawCachePutJsonGetJsonMediaTypeTest()
        {
            var df = new DataFormat();
            df.Marshaller = new TransparentMarshaller();
            df.KeyMediaType = "application/json";
            df.ValueMediaType = "application/json";
            var jsonCache = cache.WithDataFormat(df);
            jsonCache.Put("\"k1\"", "value1");
            var retVal = jsonCache.Get("\"k1\"");
            Assert.AreEqual("\"value1\"", retVal);
        }


        // Testing against a typed jboss marshalled cache. JBoss preamble is consumed to 
        // identify the type of the entry. Transparent marshaller is used to check the encoding.
        [Test]
        public void JBossCacheDefaultJbossMediaTypeTest()
        {
            transcodingCache.Put("k1", "value1");
            Assert.AreEqual("value1", transcodingCache.Get("k1"));
        }

        [Test]
        public void JBossCachePutJbossGetJsonMediaTypeTest()
        {
            var df = new DataFormat();
            df.Marshaller = new TransparentMarshaller();
            df.KeyMediaType = "application/json";
            df.ValueMediaType = "application/json";
            var jsonCache = transcodingCache.WithDataFormat(df);
            transcodingCache.Put("k1", "value1");
            var retVal = jsonCache.Get("\"k1\"");
            Assert.AreEqual("\"value1\"", retVal);
        }

        [Test]
        public void JBossCachePutJbossGetRawMediaTypeTest()
        {
            var df = new DataFormat();
            df.Marshaller = new TransparentMarshaller();
            df.KeyMediaType = "application/unknown";
            df.ValueMediaType = "application/unknown";
            var jsonCache = transcodingCache.WithDataFormat(df);
            transcodingCache.Put("k1", "value1");
            var retVal = jsonCache.Get("\x0003>\x0002k1");
            Assert.AreEqual("\x0003>\x0006value1", retVal);
        }

        [Test]
        public void JBossCachePutJbossIntegerGetJsonMediaTypeTest()
        {
            var df = new DataFormat();
            df.Marshaller = new TransparentMarshaller();
            df.KeyMediaType = "application/json";
            df.ValueMediaType = "application/json";
            var jsonCache = transcodingCache.WithDataFormat(df);
            transcodingCache.Put("k1", 42);
            var retVal = jsonCache.Get("\"k1\"");
            Assert.AreEqual("42", retVal);
        }

        [Test]
        public void JBossCachePutJsonIntegerGetJBossMediaTypeTest()
        {
            var df = new DataFormat();
            df.Marshaller = new TransparentMarshaller();
            df.KeyMediaType = "application/json";
            df.ValueMediaType = "application/json";
            var jsonCache = transcodingCache.WithDataFormat(df);
            jsonCache.Put("\"k1\"", "42");
            var retVal = transcodingCache.Get("k1");
            Assert.AreEqual(42, retVal);
        }

        [Test]
        public void GetTest()
        {
            String key = UniqueKey.NextKey();
            Assert.IsNull(cache.Get(key));
            cache.Put(key, "carbon");
            Assert.AreEqual("carbon", cache.Get(key));
        }

    }
}
