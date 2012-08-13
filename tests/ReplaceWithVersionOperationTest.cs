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
    public class ReplaceWithVersionOperationTest:SingleServerAbstractTest
    {
        [TestMethod()]
        public void replaceIfUnmodifiedTest()
        {
            RemoteCache<String,String> defaultCache = remoteManager.getCache();
            defaultCache.put("key8", "bromine1");
            long version = defaultCache.getVersioned("key8").getVersion();
            defaultCache.put("key8", "hexane");
            bool response = defaultCache.replaceWithVersion("key8", "barium", version);
            Assert.IsFalse(response);
            Assert.AreEqual("hexane", defaultCache.get("key8"));
            
            defaultCache.put("key8", "oxygen");
            long newVersion = defaultCache.getVersioned("key8").getVersion();
            Assert.AreNotEqual(newVersion, version);
            Assert.IsTrue(defaultCache.replaceWithVersion("key8", "barium", newVersion));
            Assert.AreEqual("barium", defaultCache.get("key8"));
        }
    }
}
