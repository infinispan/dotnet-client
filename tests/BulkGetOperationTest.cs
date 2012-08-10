using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using System.Collections.Generic;

namespace tests
{
    [TestClass()]
    public class BulkGetOperationTest:SingleServerAbstractTest
    {
        private RemoteCache<String,String> defaultCache;

        [TestInitialize()]
        public void populateCache()
        {
            defaultCache = remoteManager.getCache();
            defaultCache.put("key1", "hydrogen");
            defaultCache.put("key2", "helium");
            defaultCache.put("key3", "lithium");
        }

        [TestMethod()]
        public void getBulkTest()
        {
            Dictionary<String,String> actual = defaultCache.getBulk();
            Assert.AreEqual("hydrogen", actual["key1"]);
            Assert.AreEqual("helium", actual["key2"]);
            Assert.AreEqual("lithium", actual["key3"]);
        }

        [TestMethod()]
        public void getBulkTestWithSize()
        {
            Dictionary<String, String> actual = defaultCache.getBulk(2);
            Assert.AreEqual(actual.Count, 2);
        }
    }
}
