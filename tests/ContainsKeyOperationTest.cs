using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Trans.TCP;
using System.Text;

namespace tests
{

    [TestClass()]
    public class ContainsKeyOperationTest:SingleServerAbstractTest
    {
        [TestMethod()]
        public void containsKeyTest()
        {
            RemoteCache defaultCache = remoteManager.getCache();
            Assert.IsFalse(defaultCache.containsKey<String>("key4"));
            
            defaultCache.put<String, String>("key4", "oxygen");
            Assert.IsTrue(defaultCache.containsKey<String>("key4"));
        }
    }
}
