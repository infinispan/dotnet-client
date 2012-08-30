using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Infinispan.DotNetClient;

namespace tests
{
    [TestClass]
    public class NamedCacheTest : SingleServerAbstractTest
    {
        protected override void UpdateConfiguration()
        {
            configFile = "local-named-cache.xml";
            sleepTime = 15000;
        }

        [TestMethod]
        public void TestWithName()
        {
            IRemoteCache<String, String> someCache = remoteManager.GetCache<String, String>("someCache");
            someCache.Put("k2", "v2");
            Assert.AreEqual("v2", someCache.Get("k2"));

            IRemoteCache<String, String> defaultCache = remoteManager.GetCache<String, String>();
            Assert.IsNull(defaultCache.Get("k2"));
            Assert.IsFalse(defaultCache.ContainsKey("k2"));

            defaultCache.Put("k1", "v1");

            Assert.IsTrue(defaultCache.ContainsKey("k1"));

            Assert.IsFalse(someCache.ContainsKey("k1"));

        }
    }
}
