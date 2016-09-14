using System;
using Infinispan.HotRod.SWIG;

namespace Infinispan.HotRod.Config
{
#pragma warning disable 1591
    public class ServerConfigurationBuilder 
    {
        private Infinispan.HotRod.SWIG.ServerConfigurationBuilder builder;
        private ConfigurationBuilder parent;
        internal ServerConfigurationBuilder(ConfigurationBuilder parent,
                                            Infinispan.HotRod.SWIG.ServerConfigurationBuilder builder)
        {
            this.builder = builder;
            this.parent = parent;
        }

        public ServerConfiguration Create()
        {
            return new ServerConfiguration(builder.Create());
        }

        public ServerConfigurationBuilder Host(String host)
        {
            builder.Host(host);
            return this;
        }

        public ServerConfigurationBuilder Port(int port)
        {
            builder.Port(port);
            return this;
        }
    }
#pragma warning restore 1591
}