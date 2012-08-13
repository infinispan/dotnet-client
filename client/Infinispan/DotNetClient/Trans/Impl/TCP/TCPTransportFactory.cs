using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Text;
using Infinispan.DotNetClient.Util;
using Infinispan.DotNetClient.Trans;
using System.Net;
using Infinispan.DotNetClient.Exceptions;
using System.Threading;
using Infinispan.DotNetClient.Util.Impl;

namespace Infinispan.DotNetClient.Trans.Impl.TCP
{
    public class TCPTransportFactory
    {
        private IPAddress serverIP;
        private int serverPort;
        private ClientConfig config;
        private int maxTransportPoolSize;
        private int minTransportPoolSize;
        private ConnectionPool connectionPool;
        private RequestBalancer balancer;

        public TCPTransportFactory(ClientConfig configuration)
        {
            this.config = configuration;
            serverIP = IPAddress.Parse(config.ServerIP);
            serverPort = Convert.ToInt16(config.ServerPort);
            //initializeTransportPool();
        }

        public ITransport getTransport()
        {
            //IPEndPoint addr;
            //Monitor.Enter(this);
            //addr = balancer.nextServer();
            //Monitor.Exit(this);
            //return borrowTransportFromPool(addr);
            return new TCPTransport(IPAddress.Loopback, 11222);
        }

        private void createAndPreparePool(List<IPEndPoint> staticConfiguredServers)
        {
            connectionPool = ConnectionPool.getInstance();
            foreach (IPEndPoint addr in staticConfiguredServers)
            {
                connectionPool.prepareConnectionPool(addr);
            }
        }

        public void releaseTransport(ITransport transport)
        {
            ConnectionPool.getInstance().releaseTransport(transport);
        }
        
        private ITransport borrowTransportFromPool(IPEndPoint addr)
        {
            connectionPool = ConnectionPool.getInstance();
            try
            {
                return connectionPool.borrowTransport(addr);
            }
            catch (TransportException e)
            {
                throw e;
            }
        }
    }
}
