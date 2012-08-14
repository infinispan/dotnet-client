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
            RemoteCache<String, String> defaultCache = remoteManager.getCache(false);
            Assert.IsNull(defaultCache.put("k", "v"));
            Assert.IsNull(defaultCache.put("k", "v2")); //this shouldn't return anything as force return value is off

            RemoteCache<String, String> withReturnCache = remoteManager.getCache(true);
            Assert.AreEqual("v2", withReturnCache.put("k", "v3")); //this should return the previous existing value

            //test remove
            Assert.IsNull(defaultCache.remove("k"));
            withReturnCache.put("k", "v");
            Assert.AreEqual("v", withReturnCache.remove("k"));

            //test putIfAbsent
            Assert.IsNull(withReturnCache.putIfAbsent("k2","v4"));
            Assert.AreEqual("v4", withReturnCache.putIfAbsent("k2", "v5"));

            //test replace
            Assert.AreEqual("v4", withReturnCache.replace("k2","v6"));
            Assert.AreEqual("v6", withReturnCache.get("k2"));
        }
    }
}
