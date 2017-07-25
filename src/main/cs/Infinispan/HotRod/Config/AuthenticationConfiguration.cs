using Infinispan.HotRod.SWIG;

namespace Infinispan.HotRod.Config
{
    /// <summary>
    ///   Used to hold the SSL specific configurations.
    /// </summary>
    public class AuthenticationConfiguration
    {
        private Infinispan.HotRod.SWIG.AuthenticationConfiguration config;
        private System.Collections.Generic.IDictionary<int, AuthenticationCallback> callBackMap;
        internal AuthenticationConfiguration(Infinispan.HotRod.SWIG.AuthenticationConfiguration config, System.Collections.Generic.IDictionary<int, AuthenticationCallback> callBackMap)
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
