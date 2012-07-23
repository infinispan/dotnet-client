using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans.TCP;
using Infinispan.DotNetClient.Protocol;
using System.Text;
using Infinispan.DotNetClient.Util;

namespace DotNetClientTest
{
    
    
    /// <summary>
    ///This is a test class for ReplaceIfUnmodifiedOperationTest and is intended
    ///to contain all ReplaceIfUnmodifiedOperationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ReplaceIfUnmodifiedOperationTest
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
        public void executeOperationTest1()
        {
            TCPTransport trans = new TCPTransport(System.Net.IPAddress.Loopback, 11222);
            Codec codec = new Codec();
            Serializer s = new DefaultSerializer();
            byte[] key = UTF8Encoding.UTF8.GetBytes("key10");
            byte[] value = s.serialize("trinitrotoluene");

            byte[] cacheName = null; 
            int topologyId = 0; 
            Flag[] flags = null;
            
            int lifespan = 0; 
            int maxIdle = 0; 
            long version = 0;
            ReplaceIfUnmodifiedOperation target = new ReplaceIfUnmodifiedOperation(codec, key, cacheName, topologyId, flags, value, lifespan, maxIdle, version);
            Transport transport = trans;
            VersionedOperationResponse expected = null; 
            VersionedOperationResponse actual;
            actual = target.executeOperation(transport);
            Assert.AreEqual(expected, actual.isUpdated());
            
        }
    }
}
