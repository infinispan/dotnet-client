using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient.Trans;
using NLog;
using Infinispan.DotNetClient;

namespace Infinispan.DotNetClient.Operations
{
    /*
    * Insters the value to cache if the specified key is not found in the server
    *  
    * Author: sunimalr@gmail.com
    * 
    */
    public class PutIfAbsentOperation : AbstractKeyValueOperation<byte[]>
    {
        private static Logger logger;

        public PutIfAbsentOperation(Codec codec, byte[] key, byte[] cacheName,Flag[] flags, byte[] value, int lifespan, int maxIdle) :
            base(codec, key, cacheName, flags, value, lifespan, maxIdle)
        {
            logger = LogManager.GetLogger("PutIfAbsentOperation");
        }

        public byte[] ExecuteOperation(ITransport transport)
        {
            byte status = SendOperationRequest(transport, PUT_IF_ABSENT_REQUEST, PUT_IF_ABSENT_RESPONSE);
            if (logger.IsTraceEnabled)
                logger.Trace("Status = " + status);
            
            return ReturnPossiblePrevValue(transport);
        }
    }
}
