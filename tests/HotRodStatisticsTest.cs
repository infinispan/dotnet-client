using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Impl;

namespace tests
{
    [TestClass]
    public class HotRodStatisticsTest:SingleServerAbstractTest
    {
        [TestMethod]
        public void hotRodStatisticsTest()
        {
            RemoteCache<String, String> defaultCache = remoteManager.getCache();
            defaultCache.put("key7", "carbon0");
            defaultCache.put("key8", "carbon1");
            defaultCache.put("key9", "carbon2");
            Assert.AreEqual("3", defaultCache.stats().getStatistic(ServerStatistics.CURRENT_NR_OF_ENTRIES));
            defaultCache.get("key8");
            Assert.AreEqual("1", defaultCache.stats().getStatistic(ServerStatistics.HITS));
            defaultCache.get("key25");
            Assert.AreEqual("1", defaultCache.stats().getStatistic(ServerStatistics.MISSES));
            defaultCache.get("key9");
            Assert.AreEqual("3", defaultCache.stats().getStatistic(ServerStatistics.RETRIEVALS));
            defaultCache.put("key10", "carbon3");
            Assert.AreEqual("4", defaultCache.stats().getStatistic(ServerStatistics.STORES));
            defaultCache.remove("key10");
            Assert.AreEqual("1", defaultCache.stats().getStatistic(ServerStatistics.REMOVE_HITS));
            defaultCache.remove("key10");
            Assert.AreEqual("1", defaultCache.stats().getStatistic(ServerStatistics.REMOVE_MISSES));
        }
    }
}
