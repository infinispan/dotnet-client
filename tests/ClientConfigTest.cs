using Infinispan.DotNetClient.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Trans.TCP;
using Infinispan.DotNetClient.Protocol;
using System.Text;

namespace DotNetClientTest
{
   
    
    /// <summary>
    ///This is a test class for ClientConfigTest and is intended
    ///to contain all ClientConfigTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ClientConfigTest
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
        ///A test for readAttr
        ///</summary>
        [TestMethod()]
        [DeploymentItem("dotnet_client.dll")]
        public void readAttrTest()
        {           
            ClientConfig target = new ClientConfig();
            string key1 = "serverIP";
            string expected = "";
            string actual;
            actual = target.readAttr(key1);
            Assert.AreEqual(expected, actual);

            /*
            ClientConfig target = new ClientConfig();
            string actual = target.readAttr("key");
            Assert.AreEqual("", actual);
          */
        }

        /// <summary>
        ///A test for readAttr
        ///</summary>
        
    }
}
