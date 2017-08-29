using System.Collections.Generic;

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

        public AuthenticationConfigurationBuilder SetupCallback(System.Collections.Generic.IDictionary<int, AuthenticationStringCallback> map)
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

        public AuthenticationConfigurationBuilder SetupStringCallback(string user, string password, string realm)
        {
            AuthenticationStringCallback cbUser = new AuthenticationStringCallback(user);
            AuthenticationStringCallback cbPass = new AuthenticationStringCallback(password);
            AuthenticationStringCallback cbRealm = new AuthenticationStringCallback(realm);
            IDictionary<int, AuthenticationStringCallback> cbMap = new Dictionary<int, AuthenticationStringCallback>();
            cbMap.Add((int)SaslCallbackId.SASL_CB_USER, cbUser);
            cbMap.Add((int)SaslCallbackId.SASL_CB_PASS, cbPass);
            cbMap.Add((int)SaslCallbackId.SASL_CB_GETREALM, cbRealm);
            return SetupCallback(cbMap);
        }

    }
#pragma warning restore 1591
}
