using Infinispan.HotRod.Tests;
using Infinispan.HotRod.Tests.Util;
using NUnit.Framework;
using System;
using System.Collections;

namespace Infinispan.HotRod.Tests.ClusteredSaslCsXml2
{
    [SetUpFixture]
    public class AuthenticationTestSuite
    {
        HotRodServer server1;
        HotRodServer server2;

        [OneTimeSetUp]
        public void BeforeSuite()
        {
            server1 = new HotRodServer("clustered-sasl-cs.xml");
            server1.StartHotRodServer();
            string jbossHome = System.Environment.GetEnvironmentVariable("JBOSS_HOME");
            server2 = new HotRodServer("clustered-sasl-cs.xml", "-Djboss.socket.binding.port-offset=100 -Djboss.server.data.dir=" + jbossHome + "/standalone/data100", 11322);
            server2.StartHotRodServer();
        }

        [OneTimeTearDown]
        public void AfterSuite()
        {
            if (server1.IsRunning(2000))
                server1.ShutDownHotrodServer();
            if (server2.IsRunning(2000))
                server2.ShutDownHotrodServer();
        }
    }
}
