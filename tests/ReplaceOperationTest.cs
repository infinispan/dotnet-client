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
    public class ReplaceOperationTest:SingleServerAbstractTest
    {
        [TestMethod()]
        public void replaceTest()
        {
            IRemoteCache<String, String> defaultCache = remoteManager.getCache();
            defaultCache.put("key8", "bromine");
            Assert.AreEqual("bromine", defaultCache.get("key8"));

            //This Assertion will result in a "pass" only if the "forceRetrnValue" parameter of client configuration is set to true.
            Assert.AreEqual("bromine",defaultCache.replace("key8", "neon"));
            
            Assert.AreEqual("neon", defaultCache.get("key8"));
        }
    }
}
