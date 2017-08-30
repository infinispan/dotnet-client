namespace Infinispan.HotRod.Config
{
#pragma warning disable 1591
    using System;
    using SWIGGen;

    class InternalAuthenticationFunctionCallback : Infinispan.HotRod.SWIGGen.AuthenticationStringCallback
    {
        private Func<string> f_str;
        public InternalAuthenticationFunctionCallback(Func<string> f_str)
        { this.f_str = f_str; }
        public override string getString()
        {
            return f_str();
        }
    }

    public enum SaslCallbackId:int
    {
        SASL_CB_GETPATH     = 0x0003,   /* set of search path for plugin colon sepd */
        SASL_CB_USER        = 0x4001,   /* client user identity to login as */
        SASL_CB_AUTHNAME    = 0x4002,   /* client authentication name */
        SASL_CB_PASS        = 0x4004,   /* client passphrase-based secret */
        SASL_CB_GETREALM    = 0x4008    /* realm to attempt authentication in */
    }

    public class AuthenticationCallback
    {
        public AuthenticationCallback(Func<string> f_s)
        {
            iafc = new InternalAuthenticationFunctionCallback(f_s);
        }

        public AuthenticationCallback(string s)
        {
            iafc = new InternalAuthenticationFunctionCallback(()=>s);
        }

        internal InternalAuthenticationFunctionCallback iafc;
    }


#pragma warning restore 1591

}
