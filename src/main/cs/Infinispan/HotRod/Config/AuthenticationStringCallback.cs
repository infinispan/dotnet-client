namespace Infinispan.HotRod.Config
{
#pragma warning disable 1591
    using System;
    class InternalAuthenticationStringCallback : Infinispan.HotRod.SWIGGen.AuthenticationStringCallback
    {
        private string str;
        public InternalAuthenticationStringCallback(string str)
        { this.str = str; }
        public override string getString()
        {
            return str;
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
    public class AuthenticationStringCallback
    {
        public AuthenticationStringCallback(string s)
        {
            iasc = new InternalAuthenticationStringCallback(s);
        }
        internal InternalAuthenticationStringCallback iasc;
    }

#pragma warning restore 1591

}
