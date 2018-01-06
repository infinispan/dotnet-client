using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinispan.HotRod.SWIGGen;

namespace Infinispan.HotRod
{
    class InternalFailOverRequestBalancingStrategy : Infinispan.HotRod.SWIGGen.FailOverRequestBalancingStrategy
    {
        public InternalFailOverRequestBalancingStrategy(Transport.FailOverRequestBalancingStrategy strategy) : base()
        {
            this.strategy = strategy;
            this.swigCMemOwn = false;
        }
        public override void setServers(InetSocketAddressVector servers)
        {
            IList<Transport.InetSocketAddress> l = new List<Transport.InetSocketAddress>();
            foreach (var s in servers)
            {
                l.Add(fromSWIGGen(s));
            }
            strategy.setServers(l);
        }

        public override SWIGGen.InetSocketAddress nextServer(InetSocketAddressSet failedServer)
        {
            IList<Transport.InetSocketAddress> l = new List<Transport.InetSocketAddress>();
            IntPtr iter = failedServer.create_iterator_begin();
            while (failedServer.has_next(iter))
            {
                l.Add(fromSWIGGen(failedServer.get_next_key(iter)));
            }
            failedServer.destroy_iterator(iter);
            return toSWIGGen(strategy.nextServer(l));
        }

        Transport.InetSocketAddress fromSWIGGen(Infinispan.HotRod.SWIGGen.InetSocketAddress server)
        {
            Transport.InetSocketAddress ret = new Transport.InetSocketAddress();
            ret.Hostname=server.getHostname();
            ret.Port = server.getPort();
            return ret;
        }
        Infinispan.HotRod.SWIGGen.InetSocketAddress toSWIGGen(Transport.InetSocketAddress server)
        {
            return new Infinispan.HotRod.SWIGGen.InetSocketAddress(server.Hostname, server.Port);
        }
        private Transport.FailOverRequestBalancingStrategy strategy;
    }
    namespace Transport
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public class InetSocketAddress : IEquatable<InetSocketAddress>
        {
            private string hostname;
            private int port;

            public string Hostname
            {
                get
                {
                    return hostname;
                }

                set
                {
                    hostname = value;
                }
            }

            public int Port
            {
                get
                {
                    return port;
                }

                set
                {
                    port = value;
                }
            }

            bool IEquatable<InetSocketAddress>.Equals(InetSocketAddress other)
            {
                return port.Equals(other.port) && hostname.Equals(other.hostname);
            }
        }
        public interface FailOverRequestBalancingStrategy
        {
            InetSocketAddress nextServer(IList<InetSocketAddress> failedServer);
            void setServers(IList<InetSocketAddress> servers);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    }
}
