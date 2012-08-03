using Infinispan.DotNetClient.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Trans.TCP;
using Infinispan.DotNetClient.Util;
using System.Text;
using System.Diagnostics;

namespace tests
{
    
    
    /// <summary>
    ///This is a test class for RemoveOperationTest and is intended
    ///to contain all RemoveOperationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RemoveOperationTest
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
            Console.WriteLine("initialize invoked!");
            //inspired from here http://www.csharp-station.com/howto/processstart.aspx
            Process hrServer = new Process();

            hrServer.StartInfo.FileName = "bin\\startServer.bat";
            hrServer.StartInfo.WorkingDirectory = "c:\\temp\\infinispan-5.2.0.ALPHA2-all";
            hrServer.StartInfo.Arguments = "-r hotrod";

            hrServer.Start();
            bool hasExited = hrServer.HasExited;
            Console.WriteLine("Has exited? " + hasExited);

            TCPTransport trans = new TCPTransport(System.Net.IPAddress.Loopback, 11222);
            Codec codec = new Codec();
            Serializer s = new DefaultSerializer();

            byte[] key = UTF8Encoding.UTF8.GetBytes("key10");

            RemoveOperation target = new RemoveOperation(codec, key, null, 0, null);
            Transport transport = trans;
            byte[] expected = null;
            byte[] actual;
            actual = target.executeOperation(transport);
            Assert.AreEqual(expected, actual);
        }
    }
}
