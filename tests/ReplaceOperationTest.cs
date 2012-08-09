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
    public class ReplaceOperationTest:SingleServerAbstractTest
    {
        [TestMethod()]
        public void replaceTest()
        {
            RemoteCache defaultCache = remoteManager.getCache();
            defaultCache.put<String, String>("key8", "bromine");
            Assert.AreEqual("bromine", defaultCache.get<String,String>("key8"));
            defaultCache.replace<String, String>("key8", "neon");
            Assert.AreEqual("neon", defaultCache.get<String, String>("key8"));
        }
    }
}
