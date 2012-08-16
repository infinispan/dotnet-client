using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Util;

namespace tests
{
    [TestClass()]
    public class PutIFAbsentOperationTest:SingleServerAbstractTest
    {
        [TestMethod()]
        public void putIfAbsentTest()
        {
            IRemoteCache<String, String> defaultCache = remoteManager.GetCache<String,String>(true);
            defaultCache.Put("key7", "carbon0");
            defaultCache.PutIfAbsent("key7", "carbon1");
            defaultCache.PutIfAbsent("key8", "carbon2");
            Assert.AreEqual("carbon0",defaultCache.Get("key7"));
            Assert.AreEqual("carbon2", defaultCache.Get("key8"));
        }
    }
}
