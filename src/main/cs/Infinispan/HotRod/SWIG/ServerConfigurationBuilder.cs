using System;

namespace Infinispan.HotRod.SWIG
{
    internal interface ServerConfigurationBuilder
    {

        ServerConfiguration Create();
        ServerConfigurationBuilder Host(String host);
        ServerConfigurationBuilder Port(int port);
    }
}