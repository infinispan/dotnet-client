using System;
using Infinispan.HotRod.SWIG;

namespace Infinispan.HotRod.Config
{
#pragma warning disable 1591
    public class ServerConfigurationBuilder : AbstractConfigurationChildBuilder, IBuilder<ServerConfiguration>
    {
        private Infinispan.HotRod.SWIG.ServerConfigurationBuilder builder;

        internal ServerConfigurationBuilder(ConfigurationBuilder parent,
                                            Infinispan.HotRod.SWIG.ServerConfigurationBuilder builder) : base(parent)
        {
            this.builder = builder;
        }

        public void Validate()
        {
            builder.validate();
        }

        public ServerConfiguration Create()
        {
            return new ServerConfiguration(builder.Create());
        }

        public IBuilder<ServerConfiguration> Read(ServerConfiguration bean)
        {
            builder.Read(bean.Config());
            return this;
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