using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Trans.TCP;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace tests
{
    [TestClass()]
    public class PingOperationTest:SingleServerAbstractTest
    {
        [TestMethod()]
        public void pingTest()
        {
            PingOperation.PingResult expected = PingOperation.PingResult.SUCCESS;
            PingOperation.PingResult actual = remoteManager.getCache().ping();
            Assert.AreEqual(expected, actual);

            shutDownHotrodServer(); //this should make ping fail

            actual = remoteManager.getCache().ping();
            Assert.AreEqual(PingOperation.PingResult.FAIL, actual);
        }
    }
}
