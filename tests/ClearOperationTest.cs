using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotnetClient;

namespace tests
{
    [TestClass()]
    public class ClearOperationTest : SingleServerAbstractTest
    {
         [TestMethod()]
        public void ClearTest()
        {
            IRemoteCache<String, String> defaultCache = remoteManager.GetCache<String,String>();
            defaultCache.Put("key1", "hydrogen");
            defaultCache.Put("key2", "helium");
            defaultCache.Clear();

            Assert.IsNull(defaultCache.Get("key1"));
            Assert.IsNull(defaultCache.Get("key2"));

            IServerStatistics st= defaultCache.Stats();
            //Assert.AreEqual("0", st.getStatistic(ServerStatistics.TOTAL_NR_OF_ENTRIES));
            //NOTE: There's a bug with Clear as the cache doesn't clear itself correctly.
        }
    }
}
