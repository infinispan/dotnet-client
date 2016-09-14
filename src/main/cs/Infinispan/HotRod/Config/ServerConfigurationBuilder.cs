using System;
using Infinispan.HotRod.SWIG;

namespace Infinispan.HotRod.Config
{
#pragma warning disable 1591
    public class ServerConfigurationBuilder : AbstractConfigurationChildBuilder
    {
        private Infinispan.HotRod.SWIG.ServerConfigurationBuilder jniBuilder;
        internal ServerConfigurationBuilder(ConfigurationBuilder parentBuilder,
                                            Infinispan.HotRod.SWIG.ServerConfigurationBuilder jniBuilder) : base(parentBuilder)
        {
            this.jniBuilder = jniBuilder;
        }

        public ServerConfiguration Create()
        {
            return new ServerConfiguration(jniBuilder.Create());
        }

        public ServerConfigurationBuilder Host(String host)
        {
            jniBuilder.Host(host);
            return this;
        }

        public ServerConfigurationBuilder Port(int port)
        {
            jniBuilder.Port(port);
            return this;
        }
    }
#pragma warning restore 1591
}