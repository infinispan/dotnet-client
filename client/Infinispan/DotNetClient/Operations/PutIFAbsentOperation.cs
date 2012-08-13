using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient.Trans;
using NLog;
using Infinispan.DotNetClient;
using Infinispan.DotnetClient;

namespace Infinispan.DotNetClient.Operations
{
    /*
    * Insters the value to cache if the specified key is not found in the server
    *  
    * Author: sunimalr@gmail.com
    * 
    */
    public class PutIFAbsentOperation : AbstractKeyValueOperation<byte[]>
    {
        private static Logger logger;
        
        public PutIFAbsentOperation(Codec codec, byte[] key, byte[] cacheName, int topologyId, Flag[] flags, byte[] value, int lifespan, int maxIdle) :
            base(codec, key, cacheName, topologyId, flags, value, lifespan, maxIdle)
        {
            logger = LogManager.GetLogger("PutIfAbsentOperation");
        }

        public bool executeOperation(ITransport transport)
        {
            byte status = sendOperationRequest(transport, PUT_IF_ABSENT_REQUEST, PUT_IF_ABSENT_RESPONSE);
            if (logger.IsTraceEnabled)
                logger.Trace("Status = " + status);
            if (status == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
