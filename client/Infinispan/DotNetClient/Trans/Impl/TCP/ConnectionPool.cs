using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Concurrent;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Exceptions;
using Infinispan.DotNetClient.Trans.Impl;

namespace Infinispan.DotNetClient.Trans.Impl.TCP
{
    public class ConnectionPool
    {
        private Dictionary<String, HashSet<ITransport>> transportCollection;
        public ConnectionPool()
        {
            transportCollection = new Dictionary<string, HashSet<ITransport>>();
        }

        public void PrepareConnectionPool(IPEndPoint addr)
        {
            if (transportCollection.ContainsKey(addr.Address.ToString() + ":" + addr.Port))
            {
                transportCollection[addr.Address.ToString() + ":" + addr.Port].Add(new TCPTransport(addr));
            }
            else
            {
                HashSet<ITransport> newSet = new HashSet<ITransport>();
                newSet.Add(new TCPTransport(addr));
                transportCollection.Add(addr.Address.ToString() + ":" + addr.Port, newSet);
            }
        }

        public void UpdateConnectionPool(List<IPEndPoint> serversToBeAdded)
        {
            Clear();

            foreach (IPEndPoint addr in serversToBeAdded)
            {
                if (transportCollection.ContainsKey(addr.Address.ToString() + ":" + addr.Port))
                {
                    transportCollection[addr.Address.ToString() + ":" + addr.Port].Add(new TCPTransport(addr));
                }
                else
                {
                    HashSet<ITransport> newSet = new HashSet<ITransport>();
                    newSet.Add(new TCPTransport(addr));
                    transportCollection.Add(addr.Address.ToString() + ":" + addr.Port, newSet);
                }
            }
        }

        public ITransport BorrowTransport(IPEndPoint addr)
        {
            try
            {
                ITransport temp;
                if (transportCollection[addr.Address.ToString() + ":" + addr.Port].Count > 0)
                {
                    temp = transportCollection[addr.Address.ToString() + ":" + addr.Port].ElementAt(0);
                    transportCollection[addr.Address.ToString() + ":" + addr.Port].Remove(temp);
                }
                else
                {
                    temp = new TCPTransport(addr);
                }
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
                    HashSet<ITransport> newSet = new HashSet<ITransport>();
                    newSet.Add(transport);
                    transportCollection.Add(transport.IpEndPoint().Address.ToString() + ":" + transport.IpEndPoint().Port, newSet);
                }
            }
            catch (Exception e)
            {
                throw new TransportException("Failed to release transport : " + e);
            }
        }

        public void Clear()
        {
            foreach (var transportCollectionEntry in transportCollection)
            {
                foreach (var trans in transportCollectionEntry.Value)
                {
                    trans.Release();
                }
            }
            transportCollection.Clear();
        }

    }
}
