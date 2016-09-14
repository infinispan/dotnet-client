using System.Collections;
using System.Collections.Generic;
using Infinispan.HotRod.Config;

namespace Infinispan.HotRod.SWIG
{
    internal interface Configuration
    {
        bool isForceReturnValue();
        bool isTcpNoDelay();

        string getProtocolVersion();
        int getConnectionTimeout();
        int getKeySizeEstimate();
        int getSocketTimeout();
        int getValueSizeEstimate();
        int getMaxRetries();

        ConnectionPoolConfiguration ConnectionPool();
        SslConfiguration Ssl();

        IList<ServerConfiguration> Servers();
        Dictionary<string,IList<ServerConfiguration>> GetServersMapConfiguration();
    }
}