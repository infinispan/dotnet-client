using System;
using Infinispan.HotRod.SWIGGen;

namespace Infinispan.HotRod.SWIGGen
{
    internal delegate IntPtr FailOverRequestBalancingStrategyProducerDelegate();
}
namespace Infinispan.HotRod.SWIG
{
    internal interface ConfigurationBuilder
    {
        Configuration Create();
        ServerConfigurationBuilder AddServer();
        ClusterConfigurationBuilder AddCluster(string clusterName);
        ConnectionPoolConfigurationBuilder ConnectionPool();
        SslConfigurationBuilder Ssl();
        NearCacheConfigurationBuilder NearCache();
        SecurityConfigurationBuilder Security();

        void validate();
        ConfigurationBuilder AddServers(string serverList);
        ConfigurationBuilder ConnectionTimeout(int connectionTimeout);
        ConfigurationBuilder ForceReturnValues(bool forceReturnValues);
        ConfigurationBuilder KeySizeEstimate(int keySizeEstimate);
        ConfigurationBuilder ProtocolVersion(string protocolVersion);
        ConfigurationBuilder SocketTimeout(int socketTimeout);
        ConfigurationBuilder TcpNoDelay(bool tcpNoDelay);
        ConfigurationBuilder ValueSizeEstimate(int valueSizeEstimate);
        ConfigurationBuilder MaxRetries(int maxRetries);
        ConfigurationBuilder BalancingStrategyProducer(Infinispan.HotRod.Config.FailOverRequestBalancingStrategyProducerDelegate bsp);
    }
}
