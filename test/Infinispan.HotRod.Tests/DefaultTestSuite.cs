using Infinispan.HotRod.Tests;
using Infinispan.HotRod.Tests.Util;
using NUnit.Framework;
using System.Collections;

namespace Infinispan.HotRod.TestSuites
{
    public class DefaultTestSuite
    {
        HotRodServer server;

        [TestFixtureSetUp]
        public void BeforeSuite()
        {
            server = new HotRodServer("standalone.xml");
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
                suite.Add(new ConfigurationBuilderTest());
                suite.Add(new DefaultCacheTest());
                suite.Add(new DefaultCacheForceReturnValueTest());
                suite.Add(new AsyncOperationsTest());
                suite.Add(new RemoteEventTest());
                suite.Add(new NearCacheEvictionTest());
                suite.Add(new NearCacheStaleReadsTest());
                suite.Add(new NearCacheMultipleClientsTest());
                return suite;
            }
        }
    }
}
