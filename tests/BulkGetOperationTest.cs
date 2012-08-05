using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using System.Collections.Generic;

namespace tests
{
    
    
    /// <summary>
    ///This is a test class for BulkGetOperationTest and is intended
    ///to contain all BulkGetOperationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BulkGetOperationTest
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
            SingleServerAbstractTest.r.getCache().put<String, String>("key1", "hydrogen");
            SingleServerAbstractTest.r.getCache().put<String, String>("key2", "helium");
            SingleServerAbstractTest.r.getCache().put<String, String>("key3", "lithium");
        }
        
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            SingleServerAbstractTest.r.getCache().clear();
        }
        
        #endregion


        /// <summary>
        ///A test for executeOperation
        ///</summary>
        [TestMethod()]
        public void getBulkTest()
        {
            Dictionary<String,String> actual = SingleServerAbstractTest.r.getCache().getBulk<String, String>();
            Assert.AreEqual("hydrogen", actual["key1"]);
            Assert.AreEqual("helium", actual["key2"]);
            Assert.AreEqual("lithium", actual["key3"]);
        }
    }
}
