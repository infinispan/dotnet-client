using Infinispan.HotRod.Tests;
using Infinispan.HotRod.Tests.Util;
using NUnit.Framework;
using System.Collections;
using System;

namespace Infinispan.HotRod.Tests.StandaloneXml
{
    [SetUpFixture]
    public class DefaultTestSuite
    {
        HotRodServer server;

        [OneTimeSetUp]
        public void BeforeSuite()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            server = new HotRodServer("standalone.xml");
            server.StartHotRodServer();
        }
        [OneTimeTearDown]
        public void AfterSuite()
        {
            server.ShutDownHotrodServer();
        }
    }
}
