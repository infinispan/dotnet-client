using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Trans.TCP;
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
            RemoteCache defaultCache = remoteManager.getCache();
            defaultCache.put<String, String>("key8", "bromine1");
            long version = defaultCache.getVersioned<String, String>("key8").Ver;
            defaultCache.put<String, String>("key8", "hexane");
            VersionedOperationResponse response = defaultCache.removeIfUnmodified<String>("key8", version);
            Assert.AreEqual(response.getCode(), VersionedOperationResponse.RspCode.MODIFIED_KEY);
            Assert.AreEqual("hexane", defaultCache.get<String, String>("key8"));

            long newVersion = defaultCache.getVersioned<String, String>("key8").Ver;
            response = defaultCache.removeIfUnmodified<String>("key8", newVersion);

            Assert.AreEqual(response.getCode(), VersionedOperationResponse.RspCode.SUCCESS);
            Assert.AreEqual(null, defaultCache.get<String, String>("key8"));
        }
    }
}
