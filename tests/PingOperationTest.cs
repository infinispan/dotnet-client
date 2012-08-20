using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient.Trans;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Infinispan.DotNetClient;

namespace tests
{
    [TestClass()]
    public class PingOperationTest : SingleServerAbstractTest
    {
        [TestMethod()]
        public void PingTest()
        {
            PingResult expected = PingResult.SUCCESS;
            PingResult actual = remoteManager.GetCache<String, String>().Ping();
            Assert.AreEqual(expected, actual);

            Assert.AreEqual(expected, actual);
            ShutDownHotrodServer(); //this should make ping fail
            actual = remoteManager.GetCache<String, String>().Ping();
            Assert.AreEqual(PingResult.FAIL, actual);
        }
    }
}
