using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using NLog;

namespace Infinispan.DotNetClient.Operations
{
    /*
    * Replaces en existing value of an entry specified a key
    *  
    * Author: sunimalr@gmail.com
    * 
    */
    public class ReplaceOperation : AbstractKeyValueOperation<byte[]>
    {
        private static Logger logger;
        public ReplaceOperation(Codec codec, byte[] key, byte[] cacheName, Flag[] flags, byte[] value, int lifespan, int maxIdle) :
            base(codec, key, cacheName,flags, value, lifespan, maxIdle)
        {
            logger = LogManager.GetLogger("Replace Operation");
        }

        public byte[] ExecuteOperation(ITransport transport)
        {
            byte[] result = null;
            short status = SendOperationRequest(transport, REPLACE_REQUEST, REPLACE_RESPONSE);
            if (logger.IsTraceEnabled)
                logger.Trace("ReplaceOperation status : " + status);
            if (status == NO_ERROR_STATUS || status == NOT_PUT_REMOVED_REPLACED_STATUS)
            {
                result = ReturnPossiblePrevValue(transport);
            }
            return result;
        }
    }
}
