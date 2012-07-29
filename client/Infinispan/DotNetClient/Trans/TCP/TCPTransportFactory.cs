using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
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
        private int maxTransportPoolSize;
        private int minTransportPoolSize;
        private ConcurrentQueue<Transport> transportPool;
        
        public TCPTransportFactory(ClientConfig configuration)
        {
            this.config = configuration;
            serverIP = IPAddress.Parse(config.ServerIP);
            serverPort = Convert.ToInt16(config.ServerPort);
            initializeTransportPool();
        }

        public Transport getTransport()
        {
            Transport t;
            if (transportPool.IsEmpty)
            {
                t = new TCPTransport(this.serverIP, this.serverPort);
            }
            else
            {
                transportPool.TryDequeue(out t);
            }
            return t;
        }

        public void releaseTransport(Transport transport)
        {
            transportPool.Enqueue(transport);
        }

        public void initializeTransportPool()
        {
            transportPool = new ConcurrentQueue<Transport>();
            for (int i = 0; i < maxTransportPoolSize; i++)
            {
                transportPool.Enqueue(new TCPTransport(this.serverIP,this.serverPort));
            }
        }
    }
}
