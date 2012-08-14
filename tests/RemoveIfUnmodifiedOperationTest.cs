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
    public class RemoveIfUnmodifiedOperationTest:SingleServerAbstractTest
    {
        [TestMethod()]
        public void removeIfUnmodifiedTest()
        {
            RemoteCache<String, String> defaultCache = remoteManager.getCache();
            defaultCache.put("key8", "bromine1");
            long version = defaultCache.getVersioned("key8").GetVersion();
            defaultCache.put("key8", "hexane");
            VersionedOperationResponse response = defaultCache.removeIfUnmodified("key8", version);
            Assert.AreEqual(response.GetCode(), VersionedOperationResponse.RspCode.MODIFIED_KEY);
            Assert.AreEqual("hexane", defaultCache.get("key8"));

            long newVersion = defaultCache.getVersioned("key8").GetVersion();
            response = defaultCache.removeIfUnmodified("key8", newVersion);

            Assert.AreEqual(response.GetCode(), VersionedOperationResponse.RspCode.SUCCESS);
            Assert.AreEqual(null, defaultCache.get("key8"));
        }
    }
}
