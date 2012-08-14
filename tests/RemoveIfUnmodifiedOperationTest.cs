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
            IRemoteCache<String, String> defaultCache = remoteManager.GetCache<String,String>();
            defaultCache.Put("key8", "bromine1");
            long version = defaultCache.GetVersioned("key8").GetVersion();
            defaultCache.Put("key8", "hexane");
            VersionedOperationResponse response = defaultCache.RemoveIfUnmodified("key8", version);
            Assert.AreEqual(response.GetCode(), VersionedOperationResponse.RspCode.MODIFIED_KEY);
            Assert.AreEqual("hexane", defaultCache.Get("key8"));

            long newVersion = defaultCache.GetVersioned("key8").GetVersion();
            response = defaultCache.RemoveIfUnmodified("key8", newVersion);

            Assert.AreEqual(response.GetCode(), VersionedOperationResponse.RspCode.SUCCESS);
            Assert.AreEqual(null, defaultCache.Get("key8"));
        }
    }
}
