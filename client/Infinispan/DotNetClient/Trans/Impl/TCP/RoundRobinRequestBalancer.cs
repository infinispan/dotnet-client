using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Net;
using Infinispan.DotNetClient.Trans.Impl.TCP;
using NLog;

namespace Infinispan.DotNetClient.Trans.Impl.TCP
{
    public class RoundRobinRequestBalancer : IRequestBalancer
    {
        private int roundRobinCounter;
        private static Logger logger;
        private IPEndPoint[] addressArray;
        public RoundRobinRequestBalancer()
        {
            logger = LogManager.GetLogger("RoundRobinRequestBalancer");
        }

        public void SetServers(List<IPEndPoint> serverList)
        {
            addressArray = serverList.ToArray();
            if (roundRobinCounter >= addressArray.Length)
            {
                roundRobinCounter = 0;
            }
        }

        public IPEndPoint NextServer()
        {
            IPEndPoint next;

            next = GetServerByIndex(roundRobinCounter++);
            if (roundRobinCounter >= addressArray.Length)
                roundRobinCounter = 0;
            return next;
        }

        public IPEndPoint dryRunNextServer()
        {
            return GetServerByIndex(roundRobinCounter);
        }

        private IPEndPoint GetServerByIndex(int pos)
        {
            IPEndPoint server = addressArray[pos];
            if (logger.IsTraceEnabled)
            {
                logger.Trace("Returning server: " + server);
            }
            return server;
        }
    }
}
