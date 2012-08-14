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
            IRemoteCache<String, String> defaultCache = remoteManager.GetCache<String,String>();
            defaultCache.Put("key8", "bromine");
            Assert.AreEqual("bromine", defaultCache.Get("key8"));

            //This Assertion will result in a "pass" only if the "forceRetrnValue" parameter of client configuration is set to true.
            Assert.AreEqual("bromine",defaultCache.Replace("key8", "neon"));
            
            Assert.AreEqual("neon", defaultCache.Get("key8"));
        }
    }
}
