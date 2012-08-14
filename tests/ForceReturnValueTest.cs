using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Util;
using System.Text;

namespace tests
{
    [TestClass()]
    public class ForceReturnValueTest : SingleServerAbstractTest
    {
        [TestMethod()]
        public void testForceReturnValues()
        {
            //by default a cache doesn't have FORCE return value enabled
            RemoteCache<String, String> defaultCache = remoteManager.getCache();
            Assert.IsNull(defaultCache.put("k", "v"));
            Assert.IsNull(defaultCache.put("k", "v2")); //this shouldn't return anything as force return value is off

            RemoteCache<String, String> withReturnCache = remoteManager.getCache(true);
            Assert.Equals("v2", withReturnCache.put("k", "v3")); //this should return the previous existing value

            //test remove
            Assert.IsNull(defaultCache.remove("k"));
            defaultCache.put("k", "v");
            Assert.Equals("v", defaultCache.remove("k"));
        }
    }
}
