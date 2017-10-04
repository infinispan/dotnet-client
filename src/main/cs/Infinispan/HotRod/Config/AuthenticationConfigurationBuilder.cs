using System;
using System.Collections.Generic;

namespace Infinispan.HotRod.Config
{
#pragma warning disable 1591
    public class AuthenticationConfigurationBuilder: AbstractConfigurationChildBuilder
    {
        private Infinispan.HotRod.SWIG.AuthenticationConfigurationBuilder jniBuilder;
        internal System.Collections.Generic.IDictionary<int, AuthenticationCallback> callbackMap;
        internal Infinispan.HotRod.SWIGGen.SaslCallbackHandlerMap cbHandlerMap;

        internal AuthenticationConfigurationBuilder(ConfigurationBuilder parentBuilder, Infinispan.HotRod.SWIG.AuthenticationConfigurationBuilder jniBuilder) : base(parentBuilder)
        { 
            this.jniBuilder = jniBuilder;
        }

        public void Validate()
        {
        }

        public AuthenticationConfiguration Create()
        {
            return new AuthenticationConfiguration(jniBuilder.Create(), callbackMap, cbHandlerMap);
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

        public AuthenticationConfigurationBuilder SetupCallback(string user, string password, string realm)
        {
            AuthenticationCallback cbUser = new AuthenticationCallback(user);
            AuthenticationCallback cbPass = new AuthenticationCallback(password);
            AuthenticationCallback cbRealm = new AuthenticationCallback(realm);
            IDictionary<int, AuthenticationCallback> cbMap = new Dictionary<int, AuthenticationCallback>();
            cbMap.Add((int)SaslCallbackId.SASL_CB_USER, cbUser);
            cbMap.Add((int)SaslCallbackId.SASL_CB_PASS, cbPass);
            cbMap.Add((int)SaslCallbackId.SASL_CB_GETREALM, cbRealm);
            return SetupCallback(cbMap);
        }

        public AuthenticationConfigurationBuilder SetupCallback(Func<string> userFunc, Func<string> passwordFunc, Func<string> realmFunc)
        {
            AuthenticationCallback cbUser = new AuthenticationCallback(userFunc);
            AuthenticationCallback cbPass = new AuthenticationCallback(passwordFunc);
            AuthenticationCallback cbRealm = new AuthenticationCallback(realmFunc);
            IDictionary<int, AuthenticationCallback> cbMap = new Dictionary<int, AuthenticationCallback>();
            cbMap.Add((int)SaslCallbackId.SASL_CB_USER, cbUser);
            cbMap.Add((int)SaslCallbackId.SASL_CB_PASS, cbPass);
            cbMap.Add((int)SaslCallbackId.SASL_CB_GETREALM, cbRealm);
            return SetupCallback(cbMap);
        }

        AuthenticationConfigurationBuilder SetupCallback(System.Collections.Generic.IDictionary<int, AuthenticationCallback> map)
        {
            callbackMap = map;
            cbHandlerMap = new SWIGGen.SaslCallbackHandlerMap();
            foreach (int k in map.Keys)
            {
                AuthenticationCallback ac;
                map.TryGetValue(k, out ac);
                cbHandlerMap.Add(k, ac.iafc);
            }
            jniBuilder.setupCallback(cbHandlerMap);
            return this;
        }

    }
#pragma warning restore 1591
}
