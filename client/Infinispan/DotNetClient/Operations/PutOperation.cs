using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient.Exceptions;
using NLog;
using Infinispan.DotNetClient.Hotrod;

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
        public PutOperation(Codec codec, byte[] key, byte[] cacheName, int topologyId, Flag[] flags, byte[] value, int lifespan, int maxIdle) :
            base(codec, key, cacheName, topologyId, flags, value, lifespan, maxIdle)
        {
            logger = LogManager.GetLogger("PutOperation");
        }

        //Should return a byte[] after being passed to returnPossiblePrevValue(Transport t) in final implementation
        public byte[] executeOperation(Transport transport)
        {
            byte status = sendOperationRequest(transport, HotRodConstants.PUT_REQUEST, HotRodConstants.PUT_RESPONSE);
            if (status != HotRodConstants.NO_ERROR_STATUS)
            {
                InvalidResponseException e = new InvalidResponseException("Unexpected response status: " + status);
                logger.Warn(e);
                throw e;
            }
            return returnPossiblePrevValue(transport);
        }
    }
}
