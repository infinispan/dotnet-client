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
    public class ReplaceIfUnmodifiedOperationTest:SingleServerAbstractTest
    {
        [TestMethod()]
        public void replaceIfUnmodifiedTest()
        {
            RemoteCache defaultCache = remoteManager.getCache();
            defaultCache.put<String, String>("key8", "bromine1");
            long version = defaultCache.getWithVersion<String, String>("key8").Ver1;
            defaultCache.put<String, String>("key8", "hexane");
            VersionedOperationResponse response = defaultCache.replaceIfUnmodified<String, String>("key8", "barium", version);
            Assert.AreEqual(response.getCode(), VersionedOperationResponse.RspCode.MODIFIED_KEY);
            Assert.AreEqual("hexane", defaultCache.get<String, String>("key8"));
            
            defaultCache.put<String, String>("key8", "oxygen");
            long newVersion = defaultCache.getWithVersion<String, String>("key8").Ver1;
            Assert.AreNotEqual(newVersion, version);

            Assert.AreEqual(VersionedOperationResponse.RspCode.SUCCESS, defaultCache.replaceIfUnmodified<String, String>("key8", "barium", newVersion).getCode());
            Assert.AreEqual("barium", defaultCache.get<String, String>("key8"));
        }
    }
}
