using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Trans.TCP;
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
            long eaelierVer = defaultRemote.getVersioned("key45").Ver;
            defaultRemote.put("key45", "rubidium");
            
            VersionedValue actual = defaultRemote.getVersioned("key45");
            Assert.AreNotEqual(eaelierVer, actual.Ver);
            Assert.AreEqual("rubidium", actual.getValue());
        }
    }
}
