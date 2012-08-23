using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Text;
using Infinispan.DotNetClient.Util;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Trans.TCP;
using System.Net;
using Infinispan.DotNetClient.Trans.TCP;
using Infinispan.DotNetClient.Exceptions;
using System.Threading;
using NLog;

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
        private IRequestBalancer balancer;
        private IPEndPoint temp;
        private int topologyId;
        private List<IPEndPoint> servers;
        private static Logger logger;
        private ISerializer serializer;

        public TCPTransportFactory(ClientConfig configuration, ISerializer serializer, IRequestBalancer reqBalancer)
        {
            logger = LogManager.GetLogger("TCPTransportFactory");
            this.config = configuration;
            serverIP = IPAddress.Parse(config.ServerIP);
            serverPort = Convert.ToInt16(config.ServerPort);
            balancer = reqBalancer;
            this.topologyId = configuration.TopologyId;
            CreateAndPreparePool(configuration.GetServerList());
            this.serializer = serializer;

            if (logger.IsTraceEnabled)
            {
                foreach (IPEndPoint ep in configuration.GetServerList())
                    logger.Trace("server list : " + ep.Address.ToString() + ":" + ep.Port);
            }
        }

        public ISerializer GetSerializer()
        {
            return this.serializer;
        }

        public ITransport GetTransport()
        {
            IPEndPoint addr;
            lock (this)
            {
                addr = balancer.NextServer();
            }
            return BorrowTransportFromPool(addr);
        }

        private void CreateAndPreparePool(List<IPEndPoint> staticConfiguredServers)
        {
            connectionPool = new ConnectionPool();
            foreach (IPEndPoint addr in staticConfiguredServers)
            {
                logger.Trace("Adding static server " + addr.Address.ToString() + ":" + addr.Port + " to con. pool");
                connectionPool.PrepareConnectionPool(addr);
            }
            balancer.SetServers(staticConfiguredServers);
        }

        public void ReleaseTransport(ITransport transport)
        {
            connectionPool.ReleaseTransport(transport);
            temp = new IPEndPoint(transport.GetIpAddress(), transport.GetServerPort());
        }

        private ITransport BorrowTransportFromPool(IPEndPoint addr)
        {
            try
            {
                logger.Trace("Trying to borrow a transport to : " + addr.Address.ToString() + ":" + addr.Port.ToString());
                ITransport t;
                t = connectionPool.BorrowTransport(addr);
                t.SetTransportFactory(this);
                return t;
            }
            catch (Exception e)
            {
                throw new TransportException("Failed to borrow transport from pool" + e);
            }
        }

        public void UpdateServers(List<Tuple<string, int>> newServerList)
        {
            lock (this)
            {
                List<IPEndPoint> serversToBeAdded = new List<IPEndPoint>();
                IPEndPoint ipEP;
                foreach (Tuple<string, int> tuple in newServerList)
                {
                    ipEP = new IPEndPoint(IPAddress.Parse(tuple.Item1), tuple.Item2);
                    logger.Trace("Adding server " + ipEP.Address.ToString() + ":" + ipEP.Port + " to con. pool");
                    serversToBeAdded.Add(ipEP);
                }
                this.balancer.SetServers(serversToBeAdded);
                this.connectionPool.UpdateConnectionPool(serversToBeAdded);
            }
        }

        public void Destroy()
        {
            Monitor.Enter(this);
            connectionPool.Clear();
            Monitor.Exit(this);
        }

        public void UpdateHashFunction(Dictionary<IPEndPoint, HashSet<int>> servers2Hash, int numKeyOwners, short hashFunctionVersion, int hashSpace)
        {
            throw new NotImplementedException();
        }
    }
}
