using System;
using System.Net;
using System.Text;

namespace Infinispan.Hotrod.Sasl
{
    public class ExternalMechanism : SaslMechanism
    {
        private readonly string _authorizationId;

        public ExternalMechanism(NetworkCredential credential)
        {
            _authorizationId = credential?.UserName ?? string.Empty;
        }

        public override string Challenge(string base64Challenge)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(_authorizationId));
        }
    }
}
