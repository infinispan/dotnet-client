using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Trans.TCP;
using Infinispan.DotNetClient.Util;
using System.Text;

namespace tests
{
    
    
    /// <summary>
    ///This is a test class for RemoveIfUnmodifiedOperationTest and is intended
    ///to contain all RemoveIfUnmodifiedOperationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RemoveIfUnmodifiedOperationTest
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
            SingleServerAbstractTest.r.getCache().put<String, String>("key8", "bromine");
            SingleServerAbstractTest.r.getCache().put<String, String>("key8", "brominew");
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
        public void removeIfUnmodifiedTest()
        {
            long version = SingleServerAbstractTest.r.getCache().getWithVersion<String, String>("key8").Ver1;
            SingleServerAbstractTest.r.getCache().put<String, String>("key8", "hexane");
            SingleServerAbstractTest.r.getCache().removeIfUnmodified<String>("key8", version);
            Assert.IsTrue(SingleServerAbstractTest.r.getCache().containsKey<String>("key8"));
            SingleServerAbstractTest.r.getCache().removeIfUnmodified<String>("key8", version+1);
            Assert.IsFalse(SingleServerAbstractTest.r.getCache().containsKey<String>("key8"));
        }
    }
}
