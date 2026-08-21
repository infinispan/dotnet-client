using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace Infinispan.Hotrod
{
    internal class HotRodURI
    {
        public bool UseTLS { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public List<(string Host, int Port)> Servers { get; } = new();
        public string SaslMechanism { get; private set; }
        public string TrustStorePath { get; private set; }
        public string ClientCertPath { get; private set; }
        public string ClientKeyPath { get; private set; }
        public string SniHostName { get; private set; }
        public string Token { get; private set; }
        public ClientIntelligence? Intelligence { get; private set; }
        public ProtocolVersion? Version { get; private set; }

        private const int DefaultPort = 11222;

        public static HotRodURI Parse(string rawUri)
        {
            var result = new HotRodURI();

            string rest;
            if (rawUri.StartsWith("hotrods://", StringComparison.OrdinalIgnoreCase))
            {
                result.UseTLS = true;
                rest = rawUri.Substring("hotrods://".Length);
            }
            else if (rawUri.StartsWith("hotrod://", StringComparison.OrdinalIgnoreCase))
            {
                result.UseTLS = false;
                rest = rawUri.Substring("hotrod://".Length);
            }
            else
            {
                throw new ArgumentException("Unsupported scheme, expected hotrod:// or hotrods://");
            }

            // Split off query string
            string query = null;
            var qIdx = rest.IndexOf('?');
            if (qIdx >= 0)
            {
                query = rest.Substring(qIdx + 1);
                rest = rest.Substring(0, qIdx);
            }

            // Split off user info
            var atIdx = rest.IndexOf('@');
            if (atIdx >= 0)
            {
                var userInfo = rest.Substring(0, atIdx);
                rest = rest.Substring(atIdx + 1);
                var parts = userInfo.Split(':', 2);
                result.Username = Uri.UnescapeDataString(parts[0]);
                if (parts.Length > 1)
                    result.Password = Uri.UnescapeDataString(parts[1]);
            }

            // Parse comma-separated host:port pairs
            if (string.IsNullOrEmpty(rest))
                throw new ArgumentException("No servers specified in URI");

            foreach (var segment in rest.Split(','))
            {
                if (string.IsNullOrWhiteSpace(segment))
                    continue;

                var (host, port) = SplitHostPort(segment.Trim());
                result.Servers.Add((host, port));
            }

            if (result.Servers.Count == 0)
                throw new ArgumentException("No servers specified in URI");

            // Parse query parameters
            if (!string.IsNullOrEmpty(query))
            {
                foreach (var pair in query.Split('&'))
                {
                    var eq = pair.IndexOf('=');
                    if (eq < 0)
                        throw new ArgumentException($"Invalid query parameter: {pair}");

                    var key = Uri.UnescapeDataString(pair.Substring(0, eq));
                    var val = Uri.UnescapeDataString(pair.Substring(eq + 1));
                    ParseQueryParam(result, key, val);
                }
            }

            return result;
        }

        public InfinispanClient ToClient()
        {
            var client = new InfinispanClient();

            foreach (var (host, port) in Servers)
                client.AddHost(host, port);

            if (UseTLS)
            {
                client.UseTLS = true;
                if (!string.IsNullOrEmpty(TrustStorePath))
                {
                    var chain = new X509Chain();
                    chain.ChainPolicy.ExtraStore.Add(X509CertificateLoader.LoadCertificateFromFile(TrustStorePath));
                    client.CACert = chain;
                }
            }

            if (!string.IsNullOrEmpty(Username))
                client.User = Username;
            if (!string.IsNullOrEmpty(Password))
                client.Password = Password;

            if (Intelligence.HasValue)
                client.ClientIntelligence = Intelligence.Value;
            if (Version.HasValue)
                client.Version = Version.Value;

            if (!string.IsNullOrEmpty(SaslMechanism))
            {
                client.AuthMech = SaslMechanism;
            }
            else if (!string.IsNullOrEmpty(Username))
            {
                client.AuthMech = "SCRAM-SHA-256";
            }
            else if (!string.IsNullOrEmpty(Token))
            {
                client.AuthMech = "OAUTHBEARER";
            }
            else if (!string.IsNullOrEmpty(ClientCertPath))
            {
                client.AuthMech = "EXTERNAL";
            }

            return client;
        }

        private static void ParseQueryParam(HotRodURI result, string key, string value)
        {
            switch (key)
            {
                case "sasl_mechanism":
                    result.SaslMechanism = value;
                    break;
                case "trust_store_file_name":
                case "trust_ca":
                    result.TrustStorePath = value;
                    break;
                case "key_store_file_name":
                case "client_cert":
                    result.ClientCertPath = value;
                    break;
                case "key_store_password":
                case "client_key":
                    result.ClientKeyPath = value;
                    break;
                case "sni_host_name":
                case "sni_host":
                    result.SniHostName = value;
                    break;
                case "ssl_hostname_validation":
                case "verify_hostname":
                    // Accepted for compatibility; .NET TLS validation is controlled via CACert
                    break;
                case "token":
                    result.Token = value;
                    break;
                case "client_intelligence":
                    if (!Enum.TryParse<ClientIntelligence>(value.Replace("_", ""), ignoreCase: true, out var ci))
                        throw new ArgumentException($"Unknown client_intelligence value: {value}. Expected: basic, topology_aware, hash_distribution_aware");
                    result.Intelligence = ci;
                    break;
                case "protocol_version":
                case "version":
                    if (!Enum.TryParse<ProtocolVersion>(value.Replace("_", "").Replace(".", ""), ignoreCase: true, out var pv))
                        throw new ArgumentException($"Unknown protocol_version value: {value}. Expected: version31, version40, version41");
                    result.Version = pv;
                    break;
                case "connect_timeout":
                case "socket_timeout":
                case "tcp_no_delay":
                case "tcp_keep_alive":
                    // Accepted for compatibility; these are not currently configurable on the .NET client
                    break;
                default:
                    throw new ArgumentException($"Unknown URI property: {key}");
            }
        }

        private static (string Host, int Port) SplitHostPort(string s)
        {
            if (s.StartsWith('['))
            {
                var end = s.IndexOf(']');
                if (end < 0)
                    throw new ArgumentException($"Invalid IPv6 address: {s}");

                var host = s.Substring(1, end - 1);
                if (end + 1 == s.Length)
                    return (host, DefaultPort);
                if (s[end + 1] == ':')
                    return (host, int.Parse(s.Substring(end + 2)));
                throw new ArgumentException($"Invalid address: {s}");
            }

            var lastColon = s.LastIndexOf(':');
            if (lastColon < 0)
                return (s, DefaultPort);

            return (s.Substring(0, lastColon), int.Parse(s.Substring(lastColon + 1)));
        }
    }
}
