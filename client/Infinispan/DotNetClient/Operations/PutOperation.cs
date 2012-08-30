using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Exceptions;
using NLog;

namespace Infinispan.DotNetClient.Operations
{
    /*
    * Adds an entry to the cache
    *  
    * Author: sunimalr@gmail.com
    * 
    */

    public class PutOperation : AbstractKeyValueOperation<byte[]>
    {
        private static Logger logger;
        public PutOperation(Codec codec, byte[] key, byte[] cacheName,Flag[] flags, byte[] value, int lifespan, int maxIdle) :
            base(codec, key, cacheName,flags, value, lifespan, maxIdle)
        {
            logger = LogManager.GetLogger("PutOperation");
        }

        public byte[] ExecuteOperation(ITransport transport)
        {
            byte status = SendOperationRequest(transport, HotRodConstants.PUT_REQUEST, HotRodConstants.PUT_RESPONSE);
            if (status != HotRodConstants.NO_ERROR_STATUS)
            {
                InvalidResponseException e = new InvalidResponseException("Unexpected response status: " + status);
                logger.Warn(e);
                throw e;
            }
            return ReturnPossiblePrevValue(transport);
        }
    }
}
