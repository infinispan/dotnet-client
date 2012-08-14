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
            IRemoteCache<String, String> defaultCache = remoteManager.GetCache<String,String>(false);
            Assert.AreEqual("0", defaultCache.Stats().GetStatistic(ServerStatistics.CURRENT_NR_OF_ENTRIES));
            defaultCache.Put("key7", "carbon0");
            defaultCache.Put("key8", "carbon1");
            defaultCache.Put("key9", "carbon2");
            Assert.AreEqual("3", defaultCache.Stats().GetStatistic(ServerStatistics.CURRENT_NR_OF_ENTRIES));
            Assert.AreEqual("0", defaultCache.Stats().GetStatistic(ServerStatistics.HITS));
            defaultCache.Get("key8");
            Assert.AreEqual("1", defaultCache.Stats().GetStatistic(ServerStatistics.HITS));
            Assert.AreEqual("0", defaultCache.Stats().GetStatistic(ServerStatistics.MISSES));
            defaultCache.Get("key25");
            Assert.AreEqual("1", defaultCache.Stats().GetStatistic(ServerStatistics.MISSES));
            Assert.AreEqual("2", defaultCache.Stats().GetStatistic(ServerStatistics.RETRIEVALS));
            defaultCache.Get("key9");
            Assert.AreEqual("3", defaultCache.Stats().GetStatistic(ServerStatistics.RETRIEVALS));
            Assert.AreEqual("3", defaultCache.Stats().GetStatistic(ServerStatistics.STORES));
            defaultCache.Put("key10", "carbon3");
            Assert.AreEqual("4", defaultCache.Stats().GetStatistic(ServerStatistics.STORES));
            Assert.AreEqual("0", defaultCache.Stats().GetStatistic(ServerStatistics.REMOVE_HITS));
            defaultCache.Remove("key10");
            Assert.AreEqual("1", defaultCache.Stats().GetStatistic(ServerStatistics.REMOVE_HITS));
            Assert.AreEqual("0", defaultCache.Stats().GetStatistic(ServerStatistics.REMOVE_MISSES));
            defaultCache.Remove("key10");
            Assert.AreEqual("1", defaultCache.Stats().GetStatistic(ServerStatistics.REMOVE_MISSES));
        }
    }
}
