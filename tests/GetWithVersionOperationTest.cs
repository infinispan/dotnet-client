using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using System.Text;

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
            long eaelierVer = defaultRemote.getVersioned("key45").GetVersion();
            defaultRemote.put("key45", "rubidium");
            
            IVersionedValue actual = defaultRemote.getVersioned("key45");
            Assert.AreNotEqual(eaelierVer, actual.GetVersion());
            Assert.AreEqual("rubidium", actual.GetValue());
        }
    }
}
