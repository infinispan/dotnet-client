using Infinispan.HotRod.Tests;
using Infinispan.HotRod.Tests.Util;
using NUnit.Framework;
using System.Collections;
using System;

namespace Infinispan.HotRod.Tests.StandaloneCompatibilityModeXml
{
    [SetUpFixture]
    public class CompatibilityModeTestSuite
    {
        HotRodServer server;

        [OneTimeSetUp]
        public void BeforeSuite()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            server = new HotRodServer("standalone-compatibility-mode.xml");
            server.StartHotRodServer();
        }

        [OneTimeTearDown]
        public void AfterSuite()
        {
            server.ShutDownHotrodServer();
        }
    }
}
