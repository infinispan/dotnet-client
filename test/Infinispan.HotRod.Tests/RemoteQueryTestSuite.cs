using Infinispan.HotRod.Tests;
using Infinispan.HotRod.Tests.Util;
using NUnit.Framework;
using System.Collections;
using System;
namespace Infinispan.HotRod.Tests.ClusteredIndexingXml
{
    [SetUpFixture]
    public class RemoteQueryTestSuite
    {
        HotRodServer server;

        [OneTimeSetUp]
        public void BeforeSuite()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            server = new HotRodServer("clustered-indexing.xml");
            server.StartHotRodServer();
        }

        [OneTimeTearDown]
        public void AfterSuite()
        {
            server.ShutDownHotrodServer();
        }
    }
}
