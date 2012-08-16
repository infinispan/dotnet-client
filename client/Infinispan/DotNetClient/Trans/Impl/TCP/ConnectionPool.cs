using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Concurrent;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Exceptions;
using Infinispan.DotNetClient.Trans.Impl;

namespace Infinispan.DotNetClient.Trans.TCP
{
    public class ConnectionPool
    {
        private static ConnectionPool instance = null;
        private ConcurrentDictionary<String, ConcurrentBag<ITransport>> transportCollection;

        private ConnectionPool()
        {
            transportCollection = new ConcurrentDictionary<string, ConcurrentBag<ITransport>>();
        }

        public static ConnectionPool GetInstance()
        {
            if (instance == null)
            {
                instance = new ConnectionPool();
            }
            return instance;
        }

        public void PrepareConnectionPool(IPEndPoint addr)
        {
            if (transportCollection.ContainsKey(addr.Address.ToString() + ":" + addr.Port))
            {
                transportCollection[addr.Address.ToString() + ":" + addr.Port].Add(new TCPTransport(addr));
            }
            else
            {
                ConcurrentBag<ITransport> newBag = new ConcurrentBag<ITransport>();
                newBag.Add(new TCPTransport(addr));
                transportCollection.TryAdd(addr.Address.ToString() + ":" + addr.Port, newBag);
            }
        }

        public ITransport BorrowTransport(IPEndPoint addr)
        {
            try
            {
                ITransport temp;
                if (transportCollection[addr.Address.ToString() + ":" + addr.Port].Count > 0)
                {
                    transportCollection[addr.Address.ToString() + ":" + addr.Port].TryTake(out temp);
                }
                else
                {
                    temp = new TCPTransport(addr);
                }
                //temp = new TCPTransport(addr);
                return temp;
            }
            catch (Exception e)
            {
                throw new TransportException("Unable to fetch a transport! Requested transport not available : " + e);
            }
        }

        public void ReleaseTransport(ITransport transport)
        {
            try
            {
                if (transportCollection.ContainsKey(transport.IpEndPoint().Address.ToString() + ":" + transport.IpEndPoint().Port))
                {
                    transportCollection[transport.IpEndPoint().Address.ToString() + ":" + transport.IpEndPoint().Port].Add(transport);
                }
                else
                {
                    ConcurrentBag<ITransport> newBag = new ConcurrentBag<ITransport>();
                    newBag.Add(transport);
                    transportCollection.TryAdd(transport.IpEndPoint().Address.ToString() + ":" + transport.IpEndPoint().Port, newBag);
                }
            }
            catch (Exception e)
            {
                throw new TransportException("Failed to release transport : " + e);
            }
        }

        public void Clear()
        { }

    }
}
