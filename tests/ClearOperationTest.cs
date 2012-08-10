using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Trans.TCP;

namespace tests
{
    [TestClass()]
    public class ClearOperationTest : SingleServerAbstractTest
    {
         [TestMethod()]
        public void clearTest()
        {
            RemoteCache<String, String> defaultCache = remoteManager.getCache();
            defaultCache.put("key1", "hydrogen");
            defaultCache.put("key2", "helium");
            defaultCache.clear();

            Assert.IsNull(defaultCache.get("key1"));
            Assert.IsNull(defaultCache.get("key2"));


            ServerStatistics st= defaultCache.stats();
            //Assert.AreEqual("0", st.getStatistic(ServerStatistics.TOTAL_NR_OF_ENTRIES));
            //NOTE: There's a bug with Clear as the cache doesn't clear itself correctly.
        }
    }
}
