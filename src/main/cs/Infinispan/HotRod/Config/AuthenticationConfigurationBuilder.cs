namespace Infinispan.HotRod.Config
{
#pragma warning disable 1591
    public class AuthenticationConfigurationBuilder: AbstractConfigurationChildBuilder
    {
        private Infinispan.HotRod.SWIG.AuthenticationConfigurationBuilder jniBuilder;

        internal AuthenticationConfigurationBuilder(ConfigurationBuilder parentBuilder, Infinispan.HotRod.SWIG.AuthenticationConfigurationBuilder jniBuilder) : base(parentBuilder)
        { 
            this.jniBuilder = jniBuilder;
        }

        public void Validate()
        {
        }

        public AuthenticationConfiguration Create()
        {
            return new AuthenticationConfiguration(jniBuilder.Create());
        }

        public AuthenticationConfigurationBuilder Enable()
        {
            jniBuilder.Enable();
            return this;
        }

        public AuthenticationConfigurationBuilder Disable()
        {
            jniBuilder.Disable();
            return this;
        }

        public AuthenticationConfigurationBuilder SaslMechanism(string saslMechanism)
        {
            jniBuilder.SaslMechanism(saslMechanism);
            return this;
        }

        public AuthenticationConfigurationBuilder ServerFQDN(string serverFQDN)
        {
            jniBuilder.ServerFQDN(serverFQDN);
            return this;
        }

        void setupCallback(Infinispan.HotRod.SWIGGen.SaslCallbackHandlerMap map)
        {
            jniBuilder.setupCallback(map);
        }

        public AuthenticationConfigurationBuilder setupCallback(System.Collections.Generic.IDictionary<int, AuthenticationStringCallback> map)
        {
            Infinispan.HotRod.SWIGGen.SaslCallbackHandlerMap cbMap = new SWIGGen.SaslCallbackHandlerMap();
            foreach(int k in map.Keys)
            {
                AuthenticationStringCallback asc;
                map.TryGetValue(k, out asc);
                cbMap.Add(k, asc.iasc);
            }
            jniBuilder.setupCallback(cbMap);
            return this;
        }


    }
#pragma warning restore 1591
}
