using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Infinispan.Hotrod.Sasl
{
    public enum ScramHashAlgorithm
    {
        SHA1,
        SHA256,
        SHA384,
        SHA512
    }

    public class ScramMechanism : SaslMechanism
    {
        private readonly NetworkCredential _credential;
        private readonly ScramHashAlgorithm _algorithm;
        private readonly string _clientNonce;
        private string _clientFirstBare;
        private string _serverFirstMessage;
        private int _step;

        public ScramMechanism(NetworkCredential credential, ScramHashAlgorithm algorithm)
        {
            _credential = credential;
            _algorithm = algorithm;
            _clientNonce = GenerateNonce();
        }

        public override string Challenge(string base64Challenge)
        {
            switch (_step++)
            {
                case 0:
                    return ClientFirst();
                case 1:
                    return ClientFinal(base64Challenge);
                default:
                    return string.Empty;
            }
        }

        private string ClientFirst()
        {
            var userName = SaslPrep(_credential.UserName)
                .Replace("=", "=3D")
                .Replace(",", "=2C");
            _clientFirstBare = $"n={userName},r={_clientNonce}";
            var clientFirstMessage = $"n,,{_clientFirstBare}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(clientFirstMessage));
        }

        private string ClientFinal(string base64ServerFirst)
        {
            _serverFirstMessage = Encoding.UTF8.GetString(Convert.FromBase64String(base64ServerFirst));
            var attrs = ParseAttributes(_serverFirstMessage);

            var serverNonce = attrs["r"];
            if (!serverNonce.StartsWith(_clientNonce))
                throw new InvalidOperationException("Server nonce does not start with client nonce");

            var salt = Convert.FromBase64String(attrs["s"]);
            var iterations = int.Parse(attrs["i"]);

            var saltedPassword = Hi(Encoding.UTF8.GetBytes(SaslPrep(_credential.Password)), salt, iterations);
            var clientKey = ComputeHmac(saltedPassword, "Client Key");
            var storedKey = ComputeHash(clientKey);

            var clientFinalWithoutProof = $"c=biws,r={serverNonce}";
            var authMessage = $"{_clientFirstBare},{_serverFirstMessage},{clientFinalWithoutProof}";

            var clientSignature = ComputeHmac(storedKey, authMessage);
            var clientProof = Xor(clientKey, clientSignature);

            var clientFinalMessage = $"{clientFinalWithoutProof},p={Convert.ToBase64String(clientProof)}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(clientFinalMessage));
        }

        private byte[] Hi(byte[] password, byte[] salt, int iterations)
        {
            var hashName = _algorithm switch
            {
                ScramHashAlgorithm.SHA1 => HashAlgorithmName.SHA1,
                ScramHashAlgorithm.SHA256 => HashAlgorithmName.SHA256,
                ScramHashAlgorithm.SHA384 => HashAlgorithmName.SHA384,
                ScramHashAlgorithm.SHA512 => HashAlgorithmName.SHA512,
                _ => throw new NotSupportedException()
            };
            return Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashName, HmacLength());
        }

        private byte[] ComputeHmac(byte[] key, string data)
        {
            return ComputeHmac(key, Encoding.UTF8.GetBytes(data));
        }

        private byte[] ComputeHmac(byte[] key, byte[] data)
        {
            using var hmac = CreateHmac(key);
            return hmac.ComputeHash(data);
        }

        private byte[] ComputeHash(byte[] data)
        {
            using var hash = CreateHash();
            return hash.ComputeHash(data);
        }

        private HMAC CreateHmac(byte[] key)
        {
            return _algorithm switch
            {
                ScramHashAlgorithm.SHA1 => new HMACSHA1(key),
                ScramHashAlgorithm.SHA256 => new HMACSHA256(key),
                ScramHashAlgorithm.SHA384 => new HMACSHA384(key),
                ScramHashAlgorithm.SHA512 => new HMACSHA512(key),
                _ => throw new NotSupportedException()
            };
        }

        private HashAlgorithm CreateHash()
        {
            return _algorithm switch
            {
                ScramHashAlgorithm.SHA1 => SHA1.Create(),
                ScramHashAlgorithm.SHA256 => SHA256.Create(),
                ScramHashAlgorithm.SHA384 => SHA384.Create(),
                ScramHashAlgorithm.SHA512 => SHA512.Create(),
                _ => throw new NotSupportedException()
            };
        }

        private int HmacLength()
        {
            return _algorithm switch
            {
                ScramHashAlgorithm.SHA1 => 20,
                ScramHashAlgorithm.SHA256 => 32,
                ScramHashAlgorithm.SHA384 => 48,
                ScramHashAlgorithm.SHA512 => 64,
                _ => throw new NotSupportedException()
            };
        }

        private static byte[] Xor(byte[] a, byte[] b)
        {
            var result = new byte[a.Length];
            for (int i = 0; i < a.Length; i++)
                result[i] = (byte)(a[i] ^ b[i]);
            return result;
        }

        private static Dictionary<string, string> ParseAttributes(string message)
        {
            var attrs = new Dictionary<string, string>();
            foreach (var part in message.Split(','))
            {
                var idx = part.IndexOf('=');
                if (idx > 0)
                    attrs[part.Substring(0, idx)] = part.Substring(idx + 1);
            }
            return attrs;
        }

        private static string SaslPrep(string input)
        {
            return input;
        }

        private static string GenerateNonce()
        {
            var bytes = new byte[18];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
