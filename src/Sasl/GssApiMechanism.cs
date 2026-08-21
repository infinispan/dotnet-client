using System;
using System.Buffers;
using System.Net;
using System.Net.Security;

namespace Infinispan.Hotrod.Sasl
{
    public class GssApiMechanism : SaslMechanism, IDisposable
    {
        private readonly NegotiateAuthentication _auth;
        private bool _contextEstablished;

        public GssApiMechanism(NetworkCredential credential)
        {
            _auth = new NegotiateAuthentication(new NegotiateAuthenticationClientOptions
            {
                Credential = credential,
                TargetName = $"hotrod/{credential.Domain}",
                Package = "Kerberos"
            });
        }

        public override string Challenge(string base64Challenge)
        {
            byte[] incoming = string.IsNullOrEmpty(base64Challenge)
                ? null
                : Convert.FromBase64String(base64Challenge);

            var outgoing = _auth.GetOutgoingBlob(incoming, out var status);

            if (status == NegotiateAuthenticationStatusCode.Completed)
                _contextEstablished = true;
            else if (status != NegotiateAuthenticationStatusCode.ContinueNeeded)
                throw new InvalidOperationException($"GSSAPI authentication failed: {status}");

            if (_contextEstablished && (outgoing == null || outgoing.Length == 0))
            {
                // Final step: send QOP byte (no security layer, just auth)
                return Convert.ToBase64String(new byte[] { 1, 0, 0, 0 });
            }

            return outgoing != null ? Convert.ToBase64String(outgoing) : string.Empty;
        }

        public bool IsComplete => _contextEstablished;

        public void Dispose()
        {
            _auth.Dispose();
        }
    }
}
