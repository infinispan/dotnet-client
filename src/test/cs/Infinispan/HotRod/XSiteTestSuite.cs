using Infinispan.HotRod.Tests;
using Infinispan.HotRod.Tests.Util;
using NUnit.Framework;
using System;
using System.Collections;

namespace Infinispan.HotRod.TestSuites
{
    public class XSiteTestSuite
    {
        internal static HotRodServer server1;
        internal static HotRodServer server2;

        [TestFixtureSetUp]
        public void BeforeSuite()
        {
            server1 = new HotRodServer("clustered-xsite1.xml");
            server1.StartHotRodServer();
            string jbossHome = System.Environment.GetEnvironmentVariable("JBOSS_HOME");
            server2 = new HotRodServer("clustered-xsite2.xml", "-Djboss.socket.binding.port-offset=100 -Djboss.server.data.dir=" + jbossHome + "/standalone/data100", 11322);
            server2.StartHotRodServer();
        }

        [TestFixtureTearDown]
        public void AfterSuite()
        {
            if (server1.IsRunning(2000))
                server1.ShutDownHotrodServer();
            if (server2.IsRunning(2000))
                server2.ShutDownHotrodServer();
        }

        [Suite]
        public static IEnumerable Suite
        {
            get
            {
                var suite = new ArrayList();
                suite.Add(new XSiteFailoverTest());
                return suite;
            }
        }
    }
}