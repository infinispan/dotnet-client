using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Trans.TCP;
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
            RemoteCache defaultCache = remoteManager.getCache();
            defaultCache.put<String, String>("key8", "bromine");
            Assert.IsTrue(defaultCache.containsKey<String>("key8"));
            defaultCache.remove<String, String>("key8");
            Assert.IsFalse(defaultCache.containsKey<String>("key8"));
        }
    }
}
