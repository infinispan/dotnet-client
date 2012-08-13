using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Concurrent;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Exceptions;

namespace Infinispan.DotNetClient.Trans.Impl.TCP
{
    public class ConnectionPool
    {
        private static ConnectionPool instance=null;
        private ConcurrentDictionary<String,ITransport> transportCollection;
        private ConnectionPool()
        { 
        }

        public static ConnectionPool getInstance()
        {
            if (instance == null)
            {
                instance = new ConnectionPool();
            }
            return instance;
        }

        public void prepareConnectionPool(IPEndPoint addr)
        { 
            transportCollection.TryAdd(addr.Address.ToString()+":"+addr.Port,new TCPTransport(addr.Address,addr.Port));
        }

        public ITransport borrowTransport(IPEndPoint addr)
        {
            try
            {
                ITransport temp;
                transportCollection.TryRemove(addr.Address.ToString() + ":" + addr.Port, out temp);
                return temp;
            }
            catch (Exception e)
            {
                throw new TransportException("Unable to fetch a transport! Requested transport not available : "+e);
            }
        }

        public void releaseTransport(ITransport transport)
        {
            //try
            //{
            //    transportCollection.TryAdd(transport.getIpAddress().ToString() + ":" + transport.getServerPort(), transport);
            //}
            //catch (Exception e)
            //{
            //    throw new TransportException("Failed to release transport : " + e);
            //}
        }

    }
}
