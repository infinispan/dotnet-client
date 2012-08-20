using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Util;
using System.Text;
using Infinispan.DotnetClient;

namespace tests
{
    [TestClass()]
    public class RemoveOperationTest:SingleServerAbstractTest
    {
        [TestMethod()]
        public void RemoveTest()
        {
            IRemoteCache<String, String> defaultCache = remoteManager.GetCache<String,String>();
            defaultCache.Put("key8", "bromine");
            Assert.IsTrue(defaultCache.ContainsKey("key8"));
            defaultCache.Remove("key8");
            Assert.IsFalse(defaultCache.ContainsKey("key8"));
        }
    }
}
