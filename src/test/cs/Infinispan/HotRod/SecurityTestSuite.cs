using Infinispan.HotRod.Tests;
using Infinispan.HotRod.Tests.Util;
using NUnit.Framework;
using System.Collections;

namespace Infinispan.HotRod.TestSuites
{
    public class SecurityTestSuite
    {
        HotRodServer server;

        [TestFixtureSetUp]
        public void BeforeSuite()
        {
            server = new HotRodServer("standalone-hotrod-ssl.xml");
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
                suite.Add(new SSLTest());
                return suite;
            }
        }
    }
}