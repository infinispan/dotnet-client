using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using System.Text;
using Infinispan.DotNetClient.Hotrod;

namespace tests
{

    [TestClass()]
    public class ContainsKeyOperationTest:SingleServerAbstractTest
    {
        [TestMethod()]
        public void containsKeyTest()
        {
            RemoteCache<String, String> defaultCache = remoteManager.getCache();
            Assert.IsFalse(defaultCache.containsKey("key4"));
            
            defaultCache.put("key4", "oxygen");
            Assert.IsTrue(defaultCache.containsKey("key4"));
        }
    }
}
