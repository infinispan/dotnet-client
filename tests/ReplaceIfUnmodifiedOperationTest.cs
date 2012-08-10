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
            RemoteCache<String,String> defaultCache = remoteManager.getCache();
            defaultCache.put("key8", "bromine1");
            long version = defaultCache.getVersioned("key8").Ver;
            defaultCache.put("key8", "hexane");
            VersionedOperationResponse response = defaultCache.replaceIfUnmodified("key8", "barium", version);
            Assert.AreEqual(response.GetCode(), VersionedOperationResponse.RspCode.MODIFIED_KEY);
            Assert.AreEqual("hexane", defaultCache.get("key8"));
            
            defaultCache.put("key8", "oxygen");
            long newVersion = defaultCache.getVersioned("key8").Ver;
            Assert.AreNotEqual(newVersion, version);

            Assert.AreEqual(VersionedOperationResponse.RspCode.SUCCESS, defaultCache.replaceIfUnmodified("key8", "barium", newVersion).GetCode());
            Assert.AreEqual("barium", defaultCache.get("key8"));
        }
    }
}
