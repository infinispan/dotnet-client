using Infinispan.HotRod.SWIG;

namespace Infinispan.HotRod.Config
{
    /// <summary>
    ///   Used to hold the Security specific configurations.
    /// </summary>
    public class SecurityConfiguration
    {
        private Infinispan.HotRod.SWIG.SecurityConfiguration config;

        internal SecurityConfiguration(Infinispan.HotRod.SWIG.SecurityConfiguration config)
        {
            this.config = config;
        }

        internal Infinispan.HotRod.SWIG.SecurityConfiguration Config()
        {
            return config;
        }
    }
}
