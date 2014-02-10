using System;

namespace Infinispan.HotRod.SWIG
{
    internal interface ServerConfigurationBuilder
    {
        void validate();

        ServerConfiguration Create();
        void Read(ServerConfiguration bean);
        ServerConfigurationBuilder Host(String host);
        ServerConfigurationBuilder Port(int port);
    }
}