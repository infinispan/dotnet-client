using Infinispan.HotRod.SWIG;

namespace Infinispan.HotRod.Config
{
    /// <summary>
    ///   Used to hold the SSL specific configurations.
    /// </summary>
    public class NearCacheConfiguration
    {
        private Infinispan.HotRod.SWIG.NearCacheConfiguration config;

        internal NearCacheConfiguration(Infinispan.HotRod.SWIG.NearCacheConfiguration config)
        {
            this.config = config;
        }

        internal Infinispan.HotRod.SWIG.NearCacheConfiguration Config()
        {
            return config;
        }
    }
}
