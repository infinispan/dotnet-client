using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using System.Text;
using Infinispan.DotnetClient;

namespace tests
{

    [TestClass()]
    public class ContainsKeyOperationTest:SingleServerAbstractTest
    {
        [TestMethod()]
        public void ContainsKeyTest()
        {
            IRemoteCache<String, String> defaultCache = remoteManager.GetCache<String,String>();
            Assert.IsFalse(defaultCache.ContainsKey("key4"));
            
            defaultCache.Put("key4", "oxygen");
            Assert.IsTrue(defaultCache.ContainsKey("key4"));
        }
    }
}
