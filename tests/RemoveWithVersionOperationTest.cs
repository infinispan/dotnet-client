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
    public class RemoveWithVersionOperationTest:SingleServerAbstractTest
    {
        [TestMethod()]
        public void RemoveWithVersionTest()
        {
            IRemoteCache<String, String> defaultCache = remoteManager.GetCache<String,String>();
            defaultCache.Put("key8", "bromine1");
            long version = defaultCache.GetVersioned("key8").GetVersion();
            defaultCache.Put("key8", "hexane");
            Assert.IsFalse(defaultCache.RemoveWithVersion("key8", version));
            version = defaultCache.GetVersioned("key8").GetVersion();
            Assert.IsTrue(defaultCache.RemoveWithVersion("key8", version));
            Assert.IsNull(defaultCache.Get("key8"));
        }
    }
}
