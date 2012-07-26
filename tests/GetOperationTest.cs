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
    ///This is a test class for GetOperationTest and is intended
    ///to contain all GetOperationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GetOperationTest
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
            TCPTransport trans = new TCPTransport(System.Net.IPAddress.Loopback, 11222);
            Codec codec = new Codec();
            Serializer s = new DefaultSerializer();

            // byte[] key = s.serialize("11");

            byte[] key = s.serialize("key10");
            //byte[] key= UTF8Encoding.UTF8.GetBytes("key10");

            GetOperation target = new GetOperation(codec, key, null, 0, null);
            Transport transport = trans;
            string expected = "hexachlorocyclohexane 777";
            byte[] actual;
            actual = target.executeOperation(transport);
            string res = (string)(s.deserialize(actual));


            Assert.AreEqual(expected, res);
        }
    }
}
