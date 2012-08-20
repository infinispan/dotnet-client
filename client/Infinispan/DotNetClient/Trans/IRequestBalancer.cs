using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Infinispan.DotNetClient.Trans
{
    public interface IRequestBalancer
    {
        void SetServers(List<IPEndPoint> serverList);

        IPEndPoint NextServer();

        void ReleaseAddressToBalancer(IPEndPoint releasedServer);
    }
}
