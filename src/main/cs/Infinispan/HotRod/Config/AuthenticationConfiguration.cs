using Infinispan.HotRod.SWIG;

namespace Infinispan.HotRod.Config
{
    /// <summary>
    ///   Used to hold the SSL specific configurations.
    /// </summary>
    public class AuthenticationConfiguration
    {
        private Infinispan.HotRod.SWIG.AuthenticationConfiguration config;

        internal AuthenticationConfiguration(Infinispan.HotRod.SWIG.AuthenticationConfiguration config)
        {
            this.config = config;
        }

        internal Infinispan.HotRod.SWIG.AuthenticationConfiguration Config()
        {
            return config;
        }
    }
}
