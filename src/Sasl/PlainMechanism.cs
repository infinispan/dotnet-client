using System;
using System.Net;
using System.Text;

namespace Infinispan.Hotrod.Sasl
{
    public class PlainMechanism : SaslMechanism
    {
        private readonly NetworkCredential _credential;

        public PlainMechanism(NetworkCredential credential)
        {
            _credential = credential;
        }

        public override string Challenge(string base64Challenge)
        {
            var response = $"\0{_credential.UserName}\0{_credential.Password}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(response));
        }
    }
}
