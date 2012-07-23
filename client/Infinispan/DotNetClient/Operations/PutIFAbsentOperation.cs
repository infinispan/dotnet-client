using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient.Trans;
using NLog;

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

        public byte[] executeOperation(Transport transport)
        {
            byte status = sendPutOperation(transport, PUT_IF_ABSENT_REQUEST, PUT_IF_ABSENT_RESPONSE);
            if (logger.IsTraceEnabled)
                logger.Trace("Status = " + status);
            byte[] previousValue = null;
            if ((status == NO_ERROR_STATUS) || (status == NOT_PUT_REMOVED_REPLACED_STATUS))
            {
                previousValue = returnPossiblePrevValue(transport);
                if (previousValue != null)
                    return previousValue;
                else
                    return UTF8Encoding.UTF8.GetBytes("null");
            }
            else
                return UTF8Encoding.UTF8.GetBytes("null");
        }
    }
}
