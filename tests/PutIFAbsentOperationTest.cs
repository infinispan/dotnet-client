using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Trans.TCP;
using Infinispan.DotNetClient.Util;

namespace tests
{
    [TestClass()]
    public class PutIFAbsentOperationTest:SingleServerAbstractTest
    {
        [TestMethod()]
        public void putIfAbsentTest()
        {
            RemoteCache defaultCache = remoteManager.getCache();
            defaultCache.put<String, String>("key7", "carbon0");
            defaultCache.putIfAbsent<String, String>("key7", "carbon1");
            defaultCache.putIfAbsent<String, String>("key8", "carbon2");
            Assert.AreEqual("carbon0",defaultCache.get<String, String>("key7"));
            Assert.AreEqual("carbon2", defaultCache.get<String, String>("key8"));
        }
    }
}
