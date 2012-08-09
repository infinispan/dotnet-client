using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using System.Collections.Generic;
using Infinispan.DotNetClient.Trans.TCP;

namespace tests
{
    [TestClass()]
    public class StatsOperationTest:SingleServerAbstractTest
    {
        [TestMethod()]
        public void statsOperationTest()
        {
            RemoteCache defaultCache = remoteManager.getCache();
            defaultCache.put<String, String>("key7", "carbon0");
            defaultCache.put<String, String>("key8", "carbon1");
            defaultCache.put<String, String>("key9", "carbon2");
            Assert.AreEqual("3", defaultCache.stats().getStatistic(ServerStatistics.CURRENT_NR_OF_ENTRIES));
        }
    }
}
