using System.Collections;
using System.Collections.Generic;
using Infinispan.HotRod.SWIG;

namespace Infinispan.HotRod.Config
{
    /// <summary>
    ///   Used to hold the configuration parameters.
    /// </summary>
    public class Configuration
    {
        private Infinispan.HotRod.SWIG.Configuration config;
        private IMarshaller marshaller;

        internal Configuration(Infinispan.HotRod.SWIG.Configuration config, IMarshaller marshaller)
        {
            this.config = config;
            this.marshaller = marshaller;
        }

        internal Infinispan.HotRod.SWIG.Configuration Config()
        {
            return config;
        }

        /// <summary>
        ///   Retrieve the protocol version.
        /// </summary>
        ///
        /// <returns>a protocol version string</returns>
        public string ProtocolVersion()
        {
            return config.getProtocolVersion();
        }

        /// <summary>
        ///   Retrieve the configuration pool configuration.
        /// </summary>
        ///
        /// <returns>an object holding the connection pool configurations</returns>
        public ConnectionPoolConfiguration ConnectionPool()
        {
            return new ConnectionPoolConfiguration(config.ConnectionPool());
        }

        /// <summary>
        ///   Retrives the connection timeout.
        /// </summary>
        public int ConnectionTimeout()
        {
            return config.getConnectionTimeout();
        }

        /// <summary>
        ///   Retrives the state of the force return values flag.
        /// </summary>
        public bool ForceReturnValues()
        {
            return config.isForceReturnValue();
        }

        /// <summary>
        ///   Retrieves the configured estimated size for keys.
        /// </summary>
        public int KeySizeEstimate()
        {
            return config.getKeySizeEstimate();
        }

        /// <summary>
        ///   Retrieves the state of the ping on startup flag.
        /// </summary>
        public bool PingOnStartup()
        {
            return config.isPingOnStartup();
        }

        /// <summary>
        ///   Retrieves a list of objects holding the server configurations.
        /// </summary>
        public IList<ServerConfiguration> Servers()
        {
            IList<ServerConfiguration> result = new List<ServerConfiguration>();
            foreach (Infinispan.HotRod.SWIG.ServerConfiguration conf in config.Servers()) {
                result.Add(new ServerConfiguration(conf));
            }
            return result;
        }

        /// <summary>
        ///   Retrieves the value configured for socket timeouts.
        /// </summary>
        public int SocketTimeout()
        {
            return config.getSocketTimeout();
        }

        /// <summary>
        ///   Retrieves an object holding the SSL specific configurations.
        /// </summary>
        public SslConfiguration Ssl()
        {
            return new SslConfiguration(config.Ssl());
        }

        /// <summary>
        ///   Retrieves the state of the TCP no delay flag.
        /// </summary>
        public bool TcpNoDelay()
        {
            return config.isTcpNoDelay();
        }

        /// <summary>
        ///   Retrieves the configured estimates size for values.
        /// </summary>
        public int ValueSizeEstimate()
        {
            return config.getValueSizeEstimate();
        }

        /// <summary>
        ///   Retrieves the the configured marshaller.
        /// </summary>
        public IMarshaller Marshaller()
        {
            return marshaller;
        }
    }
}