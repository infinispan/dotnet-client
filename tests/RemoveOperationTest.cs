using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Util;
using System.Text;

namespace tests
{
    [TestClass()]
    public class RemoveOperationTest:SingleServerAbstractTest
    {
        [TestMethod()]
        public void removeTest()
        {
            IRemoteCache<String, String> defaultCache = remoteManager.getCache();
            defaultCache.put("key8", "bromine");
            Assert.IsTrue(defaultCache.containsKey("key8"));
            defaultCache.remove("key8");
            Assert.IsFalse(defaultCache.containsKey("key8"));
        }
    }
}
