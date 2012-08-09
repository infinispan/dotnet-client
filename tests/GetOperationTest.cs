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
    public class GetOperationTest : SingleServerAbstractTest
    {    
        [TestMethod()]
        public void getTest()
        {
            RemoteCache defaultCache = remoteManager.getCache();

            Assert.IsNull(defaultCache.get<String, String>("key7"));

            defaultCache.put<String, String>("key7", "carbon");
            Assert.AreEqual("carbon", defaultCache.get<String,String>("key7"));
        }
    }
}
