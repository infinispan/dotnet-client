using System;

namespace Infinispan.HotRod.Config
{
    /// <summary>
    ///   Used to hold the server specific configurations.
    /// </summary>
    public class ServerConfiguration
    {
        private Infinispan.HotRod.SWIG.ServerConfiguration config;

        internal ServerConfiguration(Infinispan.HotRod.SWIG.ServerConfiguration config)
        {
            this.config = config;
        }

        internal Infinispan.HotRod.SWIG.ServerConfiguration Config()
        {
            return config;
        }

        /// <summary>
        ///   Retrieves the server host.
        /// </summary>
        public String Host()
        {
            return config.getHost();
        }

        /// <summary>
        ///   Retrieves the server port.
        /// </summary>
        public int Port()
        {
            return config.getPort();
        }
    }
}