using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Net;
using Infinispan.DotNetClient.Trans.Impl.TCP;

namespace Infinispan.DotNetClient.Trans.Impl.TCP
{
    public class RoundRobinRequestBalancer : RequestBalancer
    {
        private static RoundRobinRequestBalancer instance = null;
        private ConcurrentQueue<IPEndPoint> addressQueue;
        private RoundRobinRequestBalancer()
        {
            addressQueue = new ConcurrentQueue<IPEndPoint>();
            //addressQueue.Enqueue(new IPEndPoint(IPAddress.Loopback, 11222));
        }

        public static RoundRobinRequestBalancer getInstance()
        {
            if (instance == null)
            {
                instance = new RoundRobinRequestBalancer();
            }
            return instance;
        }

        public void setServers(List<IPEndPoint> serverList)
        {
            foreach (IPEndPoint addr in serverList)
            {
                addressQueue.Enqueue(addr);
            }
        }

        public IPEndPoint nextServer()
        {
            IPEndPoint next;
            addressQueue.TryDequeue(out next);
            return next;
            //return new IPEndPoint(IPAddress.Loopback, 11222);
        }

        public void releaseAddressToBalancer(IPEndPoint releasedServer)
        {
            addressQueue.Enqueue(releasedServer);
        }
    }
}
