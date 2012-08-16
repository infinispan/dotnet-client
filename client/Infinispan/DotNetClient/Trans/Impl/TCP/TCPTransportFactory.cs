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
        private RequestBalancer balancer;
        private IPEndPoint temp;
        private int topologyId;
        private List<IPEndPoint> servers;
        private static Logger logger;
        private ISerializer serializer;

        public TCPTransportFactory(ClientConfig configuration, ISerializer serializer)
        {
            Console.WriteLine("TCP Transport Factory came");
            logger = LogManager.GetLogger("TCPTransportFactory");
            this.config = configuration;
            serverIP = IPAddress.Parse(config.ServerIP);
            serverPort = Convert.ToInt16(config.ServerPort);
            balancer = RoundRobinRequestBalancer.getInstance();
            this.topologyId = configuration.TopologyId;
            createAndPreparePool(configuration.GetServerList());
            this.serializer = serializer;

            if (logger.IsTraceEnabled)
            {
                foreach (IPEndPoint ep in configuration.GetServerList())
                    logger.Trace("server list : " + ep.Address.ToString() + ":" + ep.Port);
            }

            //initializeTransportPool();
        }

        public ISerializer getSerializer()
        {
            return this.serializer;
        }

        public ITransport getTransport()
        {
            IPEndPoint addr;
            Monitor.Enter(this);
            addr = balancer.nextServer();
            Monitor.Exit(this);
            //***********************************************************************
           // addr = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11222);
            return borrowTransportFromPool(addr);
            //return new TCPTransport(IPAddress.Loopback, 11222);
        }

        private void createAndPreparePool(List<IPEndPoint> staticConfiguredServers)
        {
            connectionPool = ConnectionPool.getInstance();
            foreach (IPEndPoint addr in staticConfiguredServers)
            {
                logger.Trace("Adding static server " + addr.Address.ToString() + ":" + addr.Port + " to con. pool");
                connectionPool.prepareConnectionPool(addr);
            }
            balancer.setServers(staticConfiguredServers);
        }

        public void releaseTransport(ITransport transport)
        {
            ConnectionPool.getInstance().releaseTransport(transport);
            temp = new IPEndPoint(transport.getIpAddress(), transport.getServerPort());
            balancer.releaseAddressToBalancer(temp);
        }

        private ITransport borrowTransportFromPool(IPEndPoint addr)
        {
            connectionPool = ConnectionPool.getInstance();
            try
            {
                logger.Trace("Trying to borrow a transport to : "+ addr.Address.ToString() + ":" + addr.Port.ToString());
                //Console.WriteLine("Trying to borrow a transport to : " + addr.Address.ToString() + ":" + addr.Port.ToString());
                ITransport t;
                t = connectionPool.borrowTransport(addr);
                t.setTransportFactory(this);
                return t;
                //return new TCPTransport(new IPEndPoint(IPAddress.Parse("127.0.0.1"),11222));
            }
            catch (Exception e)
            {
                throw new TransportException("Failed to borrow transport from pool" + e);
            }
        }

        public void updateServers(Dictionary<IPEndPoint, HashSet<int>> newServers)
        {
            /*
            Dictionary<IPEndPoint, HashSet<int>> addedServers = new Dictionary<IPEndPoint, HashSet<int>>(newServers);
            foreach (IPEndPoint ip in addedServers.Keys)
            {
                if (servers.Contains(ip))
                {
                    servers.Remove(ip);
                }
            }

            List<IPEndPoint> failedServers = new List<IPEndPoint>(servers);
            //failedServers.removeAll(newServers);

            /*
         //1. first add new servers. For servers that went down, the returned transport will fail for now
         for (SocketAddress server : addedServers) {
            log.newServerAdded(server);
            try {
               connectionPool.addObject(server);
            } catch (Exception e) {
               log.failedAddingNewServer(server, e);
            }
         }

         //2. now set the server list to the active list of servers. All the active servers (potentially together with some
         // failed servers) are in the pool now. But after this, the pool won't be asked for connections to failed servers,
         // as the balancer will only know about the active servers
         balancer.setServers(newServers);


         //3. Now just remove failed servers
         for (SocketAddress server : failedServers) {
            log.removingServer(server);
            connectionPool.clear(server);
         }

         servers = Collections.unmodifiableList(new ArrayList(newServers));
             
      }
             */
            
        }

        public void destroy()
        {
            Monitor.Enter(this);
            connectionPool.clear();
            Monitor.Exit(this);
        }

        public void updateHashFunction(Dictionary<IPEndPoint, HashSet<int>> servers2Hash, int numKeyOwners, short hashFunctionVersion, int hashSpace)
        {
            throw new NotImplementedException();
        }
    }
}
