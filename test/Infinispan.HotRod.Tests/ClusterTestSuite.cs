using Infinispan.HotRod.Tests;
using Infinispan.HotRod.Tests.Util;
using Infinispan.HotRod;
using Infinispan.HotRod.Config;
using NUnit.Framework;
using System;
using System.Collections;

namespace Infinispan.HotRod.Tests.ClusteredXml2
{
    // This class enlist all the cluster related tests.
    // These test do not have a common setup/shutdown procedure
    // so nothing more to do here
    [SetUpFixture]
    public class ClusterTestSuite
    {
        public static HotRodServer server1;
        public static HotRodServer server2;
        public static int i = 0;
        public static RemoteCacheManager getRemoteCacheManager()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222);
            conf.AddServer().Host("127.0.0.1").Port(11322)
                .ConnectionTimeout(90000).SocketTimeout(6000);
            return new RemoteCacheManager(conf.Build(), true);
        }

        public static RemoteCacheManager getRemoteCacheManager(Configuration c)
        {
            return new RemoteCacheManager(c, true);
        }

        public static void EnsureServersUp()
        {
            System.Console.WriteLine("Checking servers");
            if (server1.IsStopped())
            {
                server1 = new HotRodServer("clustered.xml");
                server1.StartHotRodServer();
            }
            if (server2.IsStopped())
            {
                string jbossHome = System.Environment.GetEnvironmentVariable("JBOSS_HOME");
                server2 = new HotRodServer("clustered.xml", "-Djboss.socket.binding.port-offset=100 -Djboss.server.data.dir=" + jbossHome + "/standalone/data100", 11322);
                server2.StartHotRodServer();
            }
        }

        [OneTimeSetUp]
        public void BeforeSuite()
        {
            // Servers startup
            string jbossHome = System.Environment.GetEnvironmentVariable("JBOSS_HOME");
            server1 = new HotRodServer("clustered.xml");
            server1.StartHotRodServer();
            server2 = new HotRodServer("clustered.xml", "-Djboss.socket.binding.port-offset=100 -Djboss.server.data.dir=" + jbossHome + "/standalone/data100", 11322);
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
