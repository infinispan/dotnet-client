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
            IRemoteCache<String,String> defaultCache = remoteManager.GetCache<String,String>();
            defaultCache.Put("key8", "bromine1");
            long version = defaultCache.GetVersioned("key8").GetVersion();
            defaultCache.Put("key8", "hexane");
            bool response = defaultCache.ReplaceWithVersion("key8", "barium", version);
            Assert.IsFalse(response);
            Assert.AreEqual("hexane", defaultCache.Get("key8"));
            
            defaultCache.Put("key8", "oxygen");
            long newVersion = defaultCache.GetVersioned("key8").GetVersion();
            Assert.AreNotEqual(newVersion, version);
            Assert.IsTrue(defaultCache.ReplaceWithVersion("key8", "barium", newVersion));
            Assert.AreEqual("barium", defaultCache.Get("key8"));
        }
    }
}
