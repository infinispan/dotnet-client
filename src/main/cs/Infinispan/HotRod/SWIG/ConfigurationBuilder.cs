using System;

namespace Infinispan.HotRod.SWIG
{
    internal interface ConfigurationBuilder
    {
        Configuration Create();
        ServerConfigurationBuilder AddServer();
        ConnectionPoolConfigurationBuilder ConnectionPool();
        SslConfigurationBuilder Ssl();
        void Read(Configuration bean);

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
    }
}