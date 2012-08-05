using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using System.Collections.Generic;
using Infinispan.DotNetClient.Trans.TCP;

namespace tests
{
    
    
    /// <summary>
    ///This is a test class for StatsOperationTest and is intended
    ///to contain all StatsOperationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StatsOperationTest
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
        [TestInitialize()]
        public void MyTestInitialize()
        {
            SingleServerAbstractTest.r.getCache().put<String, String>("key7", "carbon0");
            SingleServerAbstractTest.r.getCache().put<String, String>("key8", "carbon1");
            SingleServerAbstractTest.r.getCache().put<String, String>("key9", "carbon2");
        }
        //
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            SingleServerAbstractTest.r.getCache().clear();
        }

        #endregion


        /// <summary>
        ///A test for statsOperation
        ///</summary>
        [TestMethod()]
        public void statsOperationTest()
        {
            Assert.AreEqual("3",SingleServerAbstractTest.r.getCache().stats().getStatistic(ServerStatistics.CURRENT_NR_OF_ENTRIES));
        }
    }
}
