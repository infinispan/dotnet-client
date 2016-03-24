using System;

namespace Infinispan.HotRod.Config
{
#pragma warning disable 1591
    internal interface ConfigurationChildBuilder
    {
        ServerConfigurationBuilder AddServer();

        ConfigurationBuilder AddServers(String servers);

        ConnectionPoolConfigurationBuilder ConnectionPool();

        ConfigurationBuilder ConnectionTimeout(int connectionTimeout);

        ConfigurationBuilder ForceReturnValues(bool forceReturnValues);

        ConfigurationBuilder KeySizeEstimate(int keySizeEstimate);

        ConfigurationBuilder ProtocolVersion(String protocolVersion);

        ConfigurationBuilder SocketTimeout(int socketTimeout);

        SslConfigurationBuilder Ssl();

        ConfigurationBuilder TcpNoDelay(bool tcpNoDelay);

        ConfigurationBuilder ValueSizeEstimate(int valueSizeEstimate);

        ConfigurationBuilder MaxRetries(int maxRetries);

        Configuration Build();
    }
#pragma warning restore 1591
}