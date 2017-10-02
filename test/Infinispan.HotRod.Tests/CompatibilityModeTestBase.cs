using Infinispan.HotRod.Tests;
using Infinispan.HotRod.Tests.Util;
using NUnit.Framework;
using System.Collections;

namespace Infinispan.HotRod.TestSuites
{
    [TestFixture]
    public class CompatibilityModeTestBase
    {
        HotRodServer server;

        [OneTimeSetUp]
        public void BeforeSuite()
        {
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