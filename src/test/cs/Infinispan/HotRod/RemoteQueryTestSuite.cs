using Infinispan.HotRod.Tests;
using Infinispan.HotRod.Tests.Util;
using NUnit.Framework;
using System.Collections;

namespace Infinispan.HotRod.TestSuites
{
    public class RemoteQueryTestSuite
    {
        HotRodServer server;

        [TestFixtureSetUp]
        public void BeforeSuite()
        {
            server = new HotRodServer("clustered-indexing.xml");
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
                suite.Add(new RemoteQueryTest());
                suite.Add(new RemoteFullTextQueryTest());
                suite.Add(new ContinuousQueryTest());
                return suite;
            }
        }
    }
}