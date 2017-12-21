using System.Collections.Generic;
using Infinispan.HotRod.Config;
using NUnit.Framework;

namespace Infinispan.HotRod.Tests
{
    [TestFixture]
    public class ConfigurationBuilderTest
    {
        private ConfigurationBuilder builder;

        [SetUp]
        public void Before()
        {
            builder = new ConfigurationBuilder();
        }
        
        [Test]
        public void AddServersTest()
        {
            Configuration configuration = builder.AddServers("1.2.3.4:12345;2.3.4.5:23456").Build();

            IList<ServerConfiguration> serverConfigurations = configuration.Servers();
            Assert.AreEqual(2, serverConfigurations.Count);
            Assert.AreEqual("1.2.3.4", serverConfigurations[0].Host());
            Assert.AreEqual(12345, serverConfigurations[0].Port());
            Assert.AreEqual("2.3.4.5", serverConfigurations[1].Host());
            Assert.AreEqual(23456, serverConfigurations[1].Port());
        }

        [Test]
        public void AddServerTest()
        {
            builder.AddServer().Host("1.2.3.4").Port(12345);
            builder.AddServer().Host("2.3.4.5").Port(23456);
            Configuration configuration = builder.Build();

            IList<ServerConfiguration> serverConfigurations = configuration.Servers();
            Assert.AreEqual(2, serverConfigurations.Count);
            Assert.AreEqual("1.2.3.4", serverConfigurations[0].Host());
            Assert.AreEqual(12345, serverConfigurations[0].Port());
            Assert.AreEqual("2.3.4.5", serverConfigurations[1].Host());
            Assert.AreEqual(23456, serverConfigurations[1].Port());
        }

        [Test]
        public void ConnectionTimeoutTest()
        {
            Configuration configuration = builder
                .ConnectionTimeout(1234).Build();

            Assert.AreEqual(1234, configuration.ConnectionTimeout());
        }

        [Test]
        public void ProtocolVersionTest()
        {
            Configuration configuration = builder
                .ProtocolVersion("1234").Build();

            Assert.AreEqual("1234", configuration.ProtocolVersion());
        }

        [Test]
        public void ForceReturnValueTest()
        {
            Configuration configuration;

            configuration = builder.ForceReturnValues(true).Build();
            Assert.AreEqual(true, configuration.ForceReturnValues());

            configuration = builder.ForceReturnValues(false).Build();
            Assert.AreEqual(false, configuration.ForceReturnValues());
        }

        [Test]
        public void KeySizeEstimateTest()
        {
            Configuration configuration;

            configuration = builder.KeySizeEstimate(123).Build();
            Assert.AreEqual(123, configuration.KeySizeEstimate());
        }

        [Test]
        public void SocketTimeoutTest()
        {
            Configuration configuration;

            configuration = builder.SocketTimeout(1234).Build();
            Assert.AreEqual(1234, configuration.SocketTimeout());
        }

        [Test]
        public void TcpNoDelayTest()
        {
            Configuration configuration;

            configuration = builder.TcpNoDelay(true).Build();
            Assert.AreEqual(true, configuration.TcpNoDelay());

            configuration = builder.TcpNoDelay(false).Build();
            Assert.AreEqual(false, configuration.TcpNoDelay());
        }

        [Test]
        public void ValueSizeEstimateTest()
        {
            Configuration configuration;

            configuration = builder.ValueSizeEstimate(1234).Build();
            Assert.AreEqual(1234, configuration.ValueSizeEstimate());
        }

        [Test]
        public void LifoTest()
        {
            Configuration configuration;

            builder.ConnectionPool().Lifo(true);
            configuration= builder.Build();
            Assert.AreEqual(true, configuration.ConnectionPool().Lifo());

            builder.ConnectionPool().Lifo(false);
            configuration = builder.Build();
            Assert.AreEqual(false, configuration.ConnectionPool().Lifo());
        }

        [Test]
        public void MaxActiveTest()
        {
            Configuration configuration;
            
            builder.ConnectionPool().MaxActive(12345);
            configuration = builder.Build();

            Assert.AreEqual(12345, configuration.ConnectionPool().MaxActive());
        }

        [Test]
        public void MaxTotalTest()
        {
            Configuration configuration;
            
            builder.ConnectionPool().MaxTotal(12345);
            configuration = builder.Build();
            Assert.AreEqual(12345, configuration.ConnectionPool().MaxTotal());
        }

        [Test]
        public void MaxIdleTest()
        {
            Configuration configuration;
            
            builder.ConnectionPool().MaxIdle(12345);
            configuration = builder.Build();
            Assert.AreEqual(12345, configuration.ConnectionPool().MaxIdle());
        }

        [Test]
        public void MinIdleTest()
        {
            Configuration configuration;
            
            builder.ConnectionPool().MinIdle(12345);
            configuration = builder.Build();
            Assert.AreEqual(12345, configuration.ConnectionPool().MinIdle());
        }

        [Test]
        public void NumTestsPerEvictionRunTest()
        {
            Configuration configuration;
            
            builder.ConnectionPool().NumTestsPerEvictionRun(12345);
            configuration = builder.Build();
            Assert.AreEqual(12345, configuration.ConnectionPool().NumTestsPerEvictionRun());
        }

        [Test]
        public void TimeBetweenEvictionRunsTest()
        {
            Configuration configuration;
            
            builder.ConnectionPool().TimeBetweenEvictionRuns(12345);
            configuration = builder.Build();
            Assert.AreEqual(12345, configuration.ConnectionPool().TimeBetweenEvictionRuns());
        }

        [Test]
        public void MinEvictableIdleTimeTest()
        {
            Configuration configuration;
            
            builder.ConnectionPool().MinEvictableIdleTime(12345);
            configuration = builder.Build();
            Assert.AreEqual(12345, configuration.ConnectionPool().MinEvictableIdleTime());
        }

        [Test]
        public void TestOnBorrowTest()
        {
            Configuration configuration;
            
            builder.ConnectionPool().TestOnBorrow(true);
            configuration = builder.Build();
            Assert.AreEqual(true, configuration.ConnectionPool().TestOnBorrow());

            builder.ConnectionPool().TestOnBorrow(false);
            configuration = builder.Build();
            Assert.AreEqual(false, configuration.ConnectionPool().TestOnBorrow());
        }

        [Test]
        public void TestOnReturnTest()
        {
            Configuration configuration;
            
            builder.ConnectionPool().TestOnReturn(true);
            configuration = builder.Build();
            Assert.AreEqual(true, configuration.ConnectionPool().TestOnReturn());

            builder.ConnectionPool().TestOnReturn(false);
            configuration = builder.Build();
            Assert.AreEqual(false, configuration.ConnectionPool().TestOnReturn());
        }

        [Test]
        public void TestWhileIdleTest()
        {
            Configuration configuration;
            
            builder.ConnectionPool().TestWhileIdle(true);
            configuration = builder.Build();
            Assert.AreEqual(true, configuration.ConnectionPool().TestWhileIdle());

            builder.ConnectionPool().TestWhileIdle(false);
            configuration = builder.Build();
            Assert.AreEqual(false, configuration.ConnectionPool().TestWhileIdle());
        }

        [Test]
        public void ExhaustedActionTest()
        {
            Configuration configuration;
            
            builder.ConnectionPool().ExhaustedAction(ExhaustedAction.EXCEPTION);
            configuration = builder.Build();
            Assert.AreEqual(ExhaustedAction.EXCEPTION, configuration.ConnectionPool().ExhaustedAction());

            builder.ConnectionPool().ExhaustedAction(ExhaustedAction.WAIT);
            configuration = builder.Build();
            Assert.AreEqual(ExhaustedAction.WAIT, configuration.ConnectionPool().ExhaustedAction());

            builder.ConnectionPool().ExhaustedAction(ExhaustedAction.CREATE_NEW);
            configuration = builder.Build();
            Assert.AreEqual(ExhaustedAction.CREATE_NEW, configuration.ConnectionPool().ExhaustedAction());
        }
    }
}