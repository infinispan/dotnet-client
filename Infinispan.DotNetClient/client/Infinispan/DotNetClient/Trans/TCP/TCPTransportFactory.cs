using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Util;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Trans.TCP;
using System.Net;

namespace Infinispan.DotNetClient.Trans.TCP
{
    public class TCPTransportFactory
    {
        private IPAddress serverIP;
        private int serverPort;
        private ClientConfig config;
        
        public TCPTransportFactory(ClientConfig configuration)
        {
            this.config = configuration;
            serverIP = IPAddress.Parse(config.ServerIP);
            serverPort = Convert.ToInt16(config.ServerPort);
        }

        public Transport getTransport()
        {
            return new TCPTransport(serverIP, serverPort);
        }

        public void releaseTransport(Transport transport)
        {
        }
    }
}
