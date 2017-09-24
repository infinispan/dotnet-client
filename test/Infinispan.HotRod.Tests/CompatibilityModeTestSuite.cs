using Infinispan.HotRod.Tests;
using Infinispan.HotRod.Tests.Util;
using NUnit.Framework;
using System.Collections;

namespace Infinispan.HotRod.TestSuites
{
    public class CompatibilityModeTestSuite
    {
        HotRodServer server;

        [TestFixtureSetUp]
        public void BeforeSuite()
        {
            server = new HotRodServer("standalone-compatibility-mode.xml");
            server.StartHotRodServer();
        }

        [TestFixtureTearDown]
        public void AfterSuite()
        {
            server.ShutDownHotrodServer();
        }

        [Suite]
        public static IEnumerable Suite
        {
            get
            {
                var suite = new ArrayList();
                suite.Add(new RemoteTaskExecTest());
                return suite;
            }
        }
    }
}