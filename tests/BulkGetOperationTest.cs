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
        private IRemoteCache<String,String> defaultCache;

        [TestInitialize()]
        public void PopulateCache()
        {
            defaultCache = remoteManager.GetCache<String,String>();
            defaultCache.Put("key1", "hydrogen");
            defaultCache.Put("key2", "helium");
            defaultCache.Put("key3", "lithium");
        }

        [TestMethod()]
        public void GetBulkTest()
        {
            Dictionary<String,String> actual = defaultCache.GetBulk();
            Assert.AreEqual("hydrogen", actual["key1"]);
            Assert.AreEqual("helium", actual["key2"]);
            Assert.AreEqual("lithium", actual["key3"]);
        }

        [TestMethod()]
        public void GetBulkTestWithSize()
        {
            Dictionary<String, String> actual = defaultCache.GetBulk(2);
            Assert.AreEqual(actual.Count, 2);
        }
    }
}
