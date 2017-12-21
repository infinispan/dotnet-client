using System;
using Infinispan.HotRod.Impl;
using Infinispan.HotRod.SWIG;

namespace Infinispan.HotRod.Config
{
#pragma warning disable 1591
    public delegate Transport.FailOverRequestBalancingStrategy FailOverRequestBalancingStrategyProducerDelegate();
    public class ConfigurationBuilder
    {
        private Infinispan.HotRod.SWIG.ConfigurationBuilder builder;
        private IMarshaller marshaller = new DefaultMarshaller();

        SecurityConfigurationBuilder m_Security;
        public ConfigurationBuilder()
        {
            if (Util.Use64()) {
                builder = new Infinispan.HotRod.SWIGGen.ConfigurationBuilder();
            } else {
                builder = new Infinispan.HotRod.SWIGGen.ConfigurationBuilder();
            }
        }

        public void Validate()
        {
            builder.validate();
        }

        public Configuration Create()
        {
            return new Configuration(builder.Create(), marshaller, this.Security().Authentication().Create());
        }

        public Configuration Build()
        {
            return Create();
        }

        public ServerConfigurationBuilder AddServer()
        {
            return new ServerConfigurationBuilder(this, builder.AddServer());
        }

        public ConfigurationBuilder AddServers(String serverList)
        {
            builder.AddServers(serverList);
            return this;
        }

        public ConnectionPoolConfigurationBuilder ConnectionPool()
        {
            return new ConnectionPoolConfigurationBuilder(this, builder.ConnectionPool());
        }
    
        public ConfigurationBuilder ConnectionTimeout(int connectionTimeout)
        {
            builder.ConnectionTimeout(connectionTimeout);
            return this;
        }
    
        public ConfigurationBuilder ForceReturnValues(bool forceReturnValues)
        {
            builder.ForceReturnValues(forceReturnValues);
            return this;
        }

        public ConfigurationBuilder KeySizeEstimate(int keySizeEstimate)
        {
            builder.KeySizeEstimate(keySizeEstimate);
            return this;
        }
    
        public ConfigurationBuilder ProtocolVersion(String protocolVersion)
        {
            builder.ProtocolVersion(protocolVersion);
            return this;
        }
    
        public ConfigurationBuilder SocketTimeout(int socketTimeout)
        {
            builder.SocketTimeout(socketTimeout);
            return this;
        }
    
        public SslConfigurationBuilder Ssl()
        {
            return new SslConfigurationBuilder(this, builder.Ssl());
        }

        public SecurityConfigurationBuilder Security()
        {
            if (m_Security == null)
            {
                m_Security = new SecurityConfigurationBuilder(this, builder.Security());
            }
            return m_Security;
        }

        public NearCacheConfigurationBuilder NearCache()
        {
            return new NearCacheConfigurationBuilder(this, builder.NearCache());
        }
    
        public ConfigurationBuilder TcpNoDelay(bool tcpNoDelay)
        {
            builder.TcpNoDelay(tcpNoDelay);
            return this;
        }
    
        public ConfigurationBuilder ValueSizeEstimate(int valueSizeEstimate)
        {
            builder.ValueSizeEstimate(valueSizeEstimate);
            return this;
        }
    
        public ConfigurationBuilder MaxRetries(int maxRetries)
        {
            builder.MaxRetries(maxRetries);
            return this;
        }

        public ClusterConfigurationBuilder AddCluster(string clusterName)
        {
            return new ClusterConfigurationBuilder(this, builder.AddCluster(clusterName));
        }

        public ConfigurationBuilder BalancingStrategyProducer(Infinispan.HotRod.Config.FailOverRequestBalancingStrategyProducerDelegate d)
        {
            builder.BalancingStrategyProducer(d);
            return this;
        }

        public ConfigurationBuilder Marshaller(IMarshaller marshaller)
        {
            this.marshaller = marshaller;
            return this;
        }
 
    }
#pragma warning restore 1591
}
