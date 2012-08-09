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
    public class PingOperationTest:SingleServerAbstractTest
    {
        [TestMethod()]
        public void pingTest()
        {
            PingResult expected = PingResult.SUCCESS;
            PingResult actual = remoteManager.getCache().ping();
            Assert.AreEqual(expected, actual);

            Assert.AreEqual(expected, actual);
            shutDownHotrodServer(); //this should make ping fail
            actual = remoteManager.getCache().ping();
            Assert.AreEqual(PingResult.FAIL, actual);
        }
    }
}
