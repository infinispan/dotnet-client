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


    /// <summary>
    ///This is a test class for PingOperationTest and is intended
    ///to contain all PingOperationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PingOperationTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            SingleServerAbstractTest.MyClassInitialize(testContext);
        }
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            SingleServerAbstractTest.r.getCache().clear();
        }
        
        #endregion

        /// <summary>
        ///A test for execute
        ///</summary>
        [TestMethod()]
        public void pingTest()
        {
            PingOperation.PingResult expected = PingOperation.PingResult.SUCCESS;
            PingOperation.PingResult actual;
            actual = SingleServerAbstractTest.r.getCache().ping();
            Assert.AreEqual(expected, actual);
        }
    }
}
