using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Trans.TCP;
using Infinispan.DotNetClient.Util;

namespace tests
{
    
    
    /// <summary>
    ///This is a test class for PutIFAbsentOperationTest and is intended
    ///to contain all PutIFAbsentOperationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PutIFAbsentOperationTest
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
            
        }
        
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            SingleServerAbstractTest.r.getCache().clear();
        }
        
        #endregion


        /// <summary>
        ///A test for PutIfAbsentOperation
        ///</summary>
        [TestMethod()]
        public void putIfAbsentTest()
        {
            SingleServerAbstractTest.r.getCache().putIfAbsent<String, String>("key7", "carbon1");
            SingleServerAbstractTest.r.getCache().putIfAbsent<String, String>("key8", "carbon2");
            Assert.AreEqual("carbon0",SingleServerAbstractTest.r.getCache().get<String, String>("key7"));
            Assert.AreEqual("carbon2", SingleServerAbstractTest.r.getCache().get<String, String>("key8"));
        }
    }
}
