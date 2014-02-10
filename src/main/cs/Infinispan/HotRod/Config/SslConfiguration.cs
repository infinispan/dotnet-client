using Infinispan.HotRod.SWIG;

namespace Infinispan.HotRod.Config
{
    /// <summary>
    ///   Used to hold the SSL specific configurations.
    /// </summary>
    public class SslConfiguration
    {
        private Infinispan.HotRod.SWIG.SslConfiguration config;

        internal SslConfiguration(Infinispan.HotRod.SWIG.SslConfiguration config)
        {
            this.config = config;
        }

        internal Infinispan.HotRod.SWIG.SslConfiguration Config()
        {
            return config;
        }
    }
}