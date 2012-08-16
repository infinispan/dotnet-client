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
            IRemoteCache<String, String> defaultCache = remoteManager.GetCache<String,String>(false);
            Assert.IsNull(defaultCache.Put("k", "v"));
            Assert.IsNull(defaultCache.Put("k", "v2")); //this shouldn't return anything as force return value is off

            IRemoteCache<String, String> withReturnCache = remoteManager.GetCache<String,String>(true);
            Assert.AreEqual("v2", withReturnCache.Put("k", "v3")); //this should return the previous existing value

            //test remove
            Assert.IsNull(defaultCache.Remove("k"));
            withReturnCache.Put("k", "v");
            Assert.AreEqual("v", withReturnCache.Remove("k"));

            //test putIfAbsent
            Assert.IsNull(withReturnCache.PutIfAbsent("k2","v4"));
            Assert.AreEqual("v4", withReturnCache.PutIfAbsent("k2", "v5"));

            //test replace
            Assert.AreEqual("v4", withReturnCache.Replace("k2","v6"));
            Assert.AreEqual("v6", withReturnCache.Get("k2"));
        }
    }
}
