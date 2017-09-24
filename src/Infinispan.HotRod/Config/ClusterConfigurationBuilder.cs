using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinispan.HotRod.SWIG;
#pragma warning disable 1591
namespace Infinispan.HotRod.Config
{
    public class ClusterConfigurationBuilder : AbstractConfigurationChildBuilder
    {
        private Infinispan.HotRod.SWIG.ClusterConfigurationBuilder jniBuilder;
        internal ClusterConfigurationBuilder(ConfigurationBuilder parentBuilder, Infinispan.HotRod.SWIG.ClusterConfigurationBuilder jniBuilder)  : base(parentBuilder)
        {
            this.jniBuilder = jniBuilder;
        }

        public ClusterConfigurationBuilder AddClusterNode(string host, int port)
        {
            jniBuilder.AddClusterNode(host, port);
            return this;
        }

    }
}
#pragma warning restore 1591
