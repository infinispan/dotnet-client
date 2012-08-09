using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Infinispan.DotNetClient.Trans.Impl.TCP
{
    interface RequestBalancer
    {
        void setServers(List<IPEndPoint> serverList);

        IPEndPoint nextServer();

        void releaseAddressToBalancer(IPEndPoint releasedServer);
    }
}
