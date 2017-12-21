namespace Infinispan.HotRod.SWIG
{
    internal interface AuthenticationConfigurationBuilder
    {
        AuthenticationConfiguration Create();
        AuthenticationConfigurationBuilder Enable();
        AuthenticationConfigurationBuilder Disable();
        AuthenticationConfigurationBuilder SaslMechanism(string saslMechanism);
        AuthenticationConfigurationBuilder ServerFQDN(string serverFQDN);
        void setupCallback(Infinispan.HotRod.SWIGGen.SaslCallbackHandlerMap map);
    }
}
