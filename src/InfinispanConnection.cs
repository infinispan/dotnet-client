using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Infinispan.Hotrod
{
    public class InfinispanConnection
    {
        public InfinispanConnection(InfinispanHost host)
        {
            Host = host;
        }

        public readonly InfinispanHost Host;
        private TcpClient _tcpClient;
        private Stream _stream;

        public bool IsConnected
        {
            get
            {
                if (_tcpClient == null) return false;
                try
                {
                    var socket = _tcpClient.Client;
                    if (socket == null || !socket.Connected) return false;
                    // Poll detects graceful close: readable + 0 bytes available means peer disconnected
                    return !(socket.Poll(0, System.Net.Sockets.SelectMode.SelectRead) && socket.Available == 0);
                }
                catch
                {
                    return false;
                }
            }
        }
        public Exception LastError { get; private set; }

        public async Task<bool> ConnectAsync()
        {
            Disconnect();
            try
            {
                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(Host.Name, Host.Port);
                _stream = _tcpClient.GetStream();

                if (Host.Cluster.UseTLS)
                {
                    var sslStream = new SslStream(_stream, false, (sender, certificate, chain, sslPolicyErrors) =>
                    {
                        if (Host.Cluster.CACert == null)
                            return true;
                        bool result = Host.Cluster.CACert.Build(new X509Certificate2(certificate));
                        if (!result)
                        {
                            System.Diagnostics.Debug.WriteLine("{0}", Host.Cluster.CACert.ChainStatus);
                        }
                        return result;
                    });
                    await sslStream.AuthenticateAsClientAsync(Host.Cluster.ServiceName);
                    _stream = sslStream;
                }
                return true;
            }
            catch (Exception ex)
            {
                LastError = ex;
                Disconnect();
                return false;
            }
        }

        public void Disconnect()
        {
            _stream?.Dispose();
            _tcpClient?.Dispose();
            _stream = null;
            _tcpClient = null;
        }

        internal Stream GetStream() => _stream;

        internal void Send(CommandContext cmdCtx, Command cmd)
        {
            var stream = new HotRodStream(_stream);
            cmd.Execute(cmdCtx, this, stream);
            _stream.Flush();
        }
    }
}
