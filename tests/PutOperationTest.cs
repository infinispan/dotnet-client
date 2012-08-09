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
    public class PutOperationTest : SingleServerAbstractTest
    {   
        [TestMethod()]
        public void putTest()
        {
            remoteManager.getCache().put<String, String>("key13", "boron");
            Assert.AreEqual("boron", remoteManager.getCache().get<String, String>("key13"));
            remoteManager.getCache().put<String, String>("key14", "chlorine");
            Assert.AreEqual("chlorine", remoteManager.getCache().get<String, String>("key14"));
        }
    }
}
