using System;

namespace Infinispan.HotRod.Config
{
#pragma warning disable 1591
    public class AbstractConfigurationChildBuilder : ConfigurationChildBuilder
    {
        private ConfigurationBuilder parent;

        internal AbstractConfigurationChildBuilder(ConfigurationBuilder parent) {
            this.parent = parent;
        }

        public ClusterConfigurationBuilder AddCluster(String clusterName)
        {
            return parent.AddCluster(clusterName);
        }

        public ServerConfigurationBuilder AddServer()
        {
            return parent.AddServer();
        }
    
        public ConfigurationBuilder AddServers(String servers)
        {
            return parent.AddServers(servers);
        }
    
        public ConnectionPoolConfigurationBuilder ConnectionPool()
        {
            return parent.ConnectionPool();
        }

        public ConfigurationBuilder ConnectionTimeout(int connectionTimeout)
        {
            return parent.ConnectionTimeout(connectionTimeout);
        }

        public ConfigurationBuilder ForceReturnValues(bool forceReturnValues)
        {
            return parent.ForceReturnValues(forceReturnValues);
        }
    
        public ConfigurationBuilder KeySizeEstimate(int keySizeEstimate)
        {
            return parent.KeySizeEstimate(keySizeEstimate);
        }
    
        public ConfigurationBuilder ProtocolVersion(String protocolVersion)
        {
            return parent.ProtocolVersion(protocolVersion);
        }
        
        public NearCacheConfigurationBuilder NearCache()
        {
            return parent.NearCache();
        }
    
        public ConfigurationBuilder SocketTimeout(int socketTimeout)
        {
            return parent.SocketTimeout(socketTimeout);
        }
    
        public SslConfigurationBuilder Ssl()
        {
            return parent.Ssl();
        }
    
        public ConfigurationBuilder TcpNoDelay(bool tcpNoDelay)
        {
            return parent.TcpNoDelay(tcpNoDelay);
        }
    
        public ConfigurationBuilder ValueSizeEstimate(int valueSizeEstimate)
        {
            return parent.ValueSizeEstimate(valueSizeEstimate);
        }
    
        public ConfigurationBuilder MaxRetries(int maxRetries)
        {
            return parent.MaxRetries(maxRetries);
        }

        public Configuration Build()
        {
            return parent.Build();
        }
    }
#pragma warning restore 1591
}