using System;
using System.Net;

namespace Infinispan.Hotrod.Sasl
{
    public abstract class SaslMechanism
    {
        public abstract string Challenge(string base64Challenge);

        public static SaslMechanism Create(string mechanism, NetworkCredential credential)
        {
            switch (mechanism)
            {
                case "PLAIN":
                    return new PlainMechanism(credential);
                case "SCRAM-SHA-1":
                    return new ScramMechanism(credential, ScramHashAlgorithm.SHA1);
                case "SCRAM-SHA-256":
                    return new ScramMechanism(credential, ScramHashAlgorithm.SHA256);
                case "SCRAM-SHA-384":
                    return new ScramMechanism(credential, ScramHashAlgorithm.SHA384);
                case "SCRAM-SHA-512":
                    return new ScramMechanism(credential, ScramHashAlgorithm.SHA512);
                default:
                    throw new NotSupportedException($"SASL mechanism '{mechanism}' is not supported");
            }
        }
    }
}
