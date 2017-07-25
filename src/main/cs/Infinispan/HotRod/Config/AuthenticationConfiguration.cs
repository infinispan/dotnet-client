using Infinispan.HotRod.SWIG;

namespace Infinispan.HotRod.Config
{
    /// <summary>
    ///   Used to hold the SSL specific configurations.
    /// </summary>
    public class AuthenticationConfiguration
    {
        private Infinispan.HotRod.SWIG.AuthenticationConfiguration config;
        private System.Collections.Generic.IDictionary<int, AuthenticationStringCallback> callBackMap;
        internal AuthenticationConfiguration(Infinispan.HotRod.SWIG.AuthenticationConfiguration config, System.Collections.Generic.IDictionary<int, AuthenticationStringCallback> callBackMap)
        {
            this.config = config;
            this.callBackMap = callBackMap;
        }

        internal Infinispan.HotRod.SWIG.AuthenticationConfiguration Config()
        {
            return config;
        }
    }
}
