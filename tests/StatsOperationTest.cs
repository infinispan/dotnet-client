using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using System.Collections.Generic;
using Infinispan.DotNetClient.Impl;

namespace tests
{
    [TestClass()]
    public class StatsOperationTest:SingleServerAbstractTest
    {
        [TestMethod()]
        public void statsOperationTest()
        {
            IRemoteCache<String, String> defaultCache = remoteManager.GetCache<String,String>();
            defaultCache.Put("key7", "carbon0");
            defaultCache.Put("key8", "carbon1");
            defaultCache.Put("key9", "carbon2");
            Assert.AreEqual("3", defaultCache.Stats().GetStatistic(ServerStatistics.CURRENT_NR_OF_ENTRIES));
        }
    }
}
