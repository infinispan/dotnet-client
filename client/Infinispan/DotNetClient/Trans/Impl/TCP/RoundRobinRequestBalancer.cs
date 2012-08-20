using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Net;
using Infinispan.DotNetClient.Trans.Impl.TCP;

namespace Infinispan.DotNetClient.Trans.Impl.TCP
{
    public class RoundRobinRequestBalancer : IRequestBalancer
    {
        //private ConcurrentQueue<IPEndPoint> addressQueue;
        private Queue<IPEndPoint> addressQueue;
        public RoundRobinRequestBalancer()
        {
            addressQueue = new Queue<IPEndPoint>();
            //addressQueue.Enqueue(new IPEndPoint(IPAddress.Loopback, 11222));
        }

        public void SetServers(List<IPEndPoint> serverList)
        {
            this.addressQueue.Clear();
            foreach (IPEndPoint addr in serverList)
            {
                addressQueue.Enqueue(addr);
            }
        }

        public IPEndPoint NextServer()
        {
            IPEndPoint next;
            next=addressQueue.Dequeue();
            return next;
            //return new IPEndPoint(IPAddress.Loopback, 11222);
        }

        public void ReleaseAddressToBalancer(IPEndPoint releasedServer)
        {
            addressQueue.Enqueue(releasedServer);
        }
    }
}
