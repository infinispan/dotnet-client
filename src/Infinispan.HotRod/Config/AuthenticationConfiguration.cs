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
        private Infinispan.HotRod.SWIGGen.SaslCallbackHandlerMap cbHandlerMap;
        internal AuthenticationConfiguration(Infinispan.HotRod.SWIG.AuthenticationConfiguration config, System.Collections.Generic.IDictionary<int, AuthenticationCallback> callBackMap, Infinispan.HotRod.SWIGGen.SaslCallbackHandlerMap cbHandlerMap)
        {
            this.config = config;
            this.callBackMap = callBackMap;
            this.cbHandlerMap = cbHandlerMap;
        }

        internal Infinispan.HotRod.SWIG.AuthenticationConfiguration Config()
        {
            return config;
        }
    }
}
