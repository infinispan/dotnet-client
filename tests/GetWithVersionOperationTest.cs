using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using System.Text;
using Infinispan.DotNetClient.Hotrod;
using Infinispan.DotNetClient.Hotrod.Impl;

namespace tests
{
    [TestClass()]
    public class GetWithVersionOperationTest : SingleServerAbstractTest
    {
        [TestMethod()]
        public void getWithVersionTest()
        {
            RemoteCache<String, String> defaultRemote = remoteManager.getCache();
            defaultRemote.put("key45", "uranium");
            long eaelierVer = defaultRemote.getVersioned("key45").getVersion();
            defaultRemote.put("key45", "rubidium");
            
            VersionedValue actual = defaultRemote.getVersioned("key45");
            Assert.AreNotEqual(eaelierVer, actual.getVersion());
            Assert.AreEqual("rubidium", actual.getValue());
        }
    }
}
