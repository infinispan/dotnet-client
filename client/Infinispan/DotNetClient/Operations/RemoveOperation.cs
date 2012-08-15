using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient.Trans;
using NLog;
using Infinispan.DotnetClient;

namespace Infinispan.DotNetClient.Operations
{
    /*
    * Remoevs entry with a given key
    *  
    * Author: sunimalr@gmail.com
    * 
    */

    public class RemoveOperation : AbstractKeyOperation<byte[]>
    {
        private static Logger logger;

        public RemoveOperation(Codec codec, byte[] key, byte[] cacheName, int topologyId, Flag[] flags) :
            base(codec, key, cacheName, topologyId, flags)
        {
            logger = LogManager.GetLogger("RemoveOperation");
        }

        public byte[] ExecuteOperation(ITransport transport)
        {
            byte[] result = null;
            byte status = SendKeyOperation(key, transport, REMOVE_REQUEST, REMOVE_RESPONSE);
            if (logger.IsTraceEnabled)
                logger.Trace("RemoveOperation status : " + status);
            if (status == KEY_DOES_NOT_EXIST_STATUS)
            {
                result = null;  //key does not exist
            }
            else if (status == NO_ERROR_STATUS)
            {
                result = ReturnPossiblePrevValue(transport);
                // result = 0;
            }
            return result;
        }
    }

}

