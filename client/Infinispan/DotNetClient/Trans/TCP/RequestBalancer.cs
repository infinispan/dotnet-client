using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Infinispan.DotNetClient.Trans.TCP
{
    public class RequestBalancer
    {
         private static RequestBalancer instance=null;
        private RequestBalancer()
        { 
        }

        public static RequestBalancer getInstance()
        {
            if (instance == null)
            {
                instance = new RequestBalancer();
            }
            return instance;
        }

        public void setServers()
        { }

        public IPEndPoint nextServer()
        {
            return new IPEndPoint(IPAddress.Loopback, 11222);
        }
    }
}
