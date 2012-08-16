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
        public void GetWithVersionTest()
        {
            IRemoteCache<String, String> defaultRemote = remoteManager.GetCache<String,String>();
            defaultRemote.Put("key45", "uranium");
            long eaelierVer = defaultRemote.GetVersioned("key45").GetVersion();
            defaultRemote.Put("key45", "rubidium");
            
            IVersionedValue actual = defaultRemote.GetVersioned("key45");
            Assert.AreNotEqual(eaelierVer, actual.GetVersion());
            Assert.AreEqual("rubidium", actual.GetValue());
        }
    }
}
