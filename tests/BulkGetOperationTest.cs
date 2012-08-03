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
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
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
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for executeOperation
        ///</summary>
        [TestMethod()]
        public void executeOperationTest()
        {
            Console.WriteLine("Mircea Markus");
            Codec codec = null; // TODO: Initialize to an appropriate value
            byte[] cacheName = null; // TODO: Initialize to an appropriate value
            int topologyId = 0; // TODO: Initialize to an appropriate value
            Flag[] flags = null; // TODO: Initialize to an appropriate value
            int entryCount = 0; // TODO: Initialize to an appropriate value
            BulkGetOperation target = new BulkGetOperation(codec, cacheName, topologyId, flags, entryCount); // TODO: Initialize to an appropriate value
            Transport transport = null; // TODO: Initialize to an appropriate value
            Dictionary<byte[], byte[]> expected = null; // TODO: Initialize to an appropriate value
            Dictionary<byte[], byte[]> actual;
            actual = target.executeOperation(transport);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
