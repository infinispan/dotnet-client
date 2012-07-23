using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Protocol;
using NLog;
using Infinispan.DotNetClient.Trans;


namespace Infinispan.DotNetClient.Operations
{
    /*
    * Retrieves a value with it's version 
    *  
    * Author: sunimalr@gmail.com
    * 
    */

    public class GetWithVersionOperation : AbstractKeyOperation<BinaryVersionedValue>
    {
        private static Logger logger;
        public GetWithVersionOperation(Codec codec, byte[] key, byte[] cacheName, int topologyId, Flag[] flags) :
            base(codec, key, cacheName, topologyId, flags)
        {
            logger = LogManager.GetLogger("GetWithVersionOperation");
        }

        public BinaryVersionedValue executeOperation(Transport transport)
        {
            short status = sendKeyOperation(key, transport, GET_WITH_VERSION, GET_WITH_VERSION_RESPONSE);
            BinaryVersionedValue result = null;
            if (status == KEY_DOES_NOT_EXIST_STATUS)
            {
                result = null;
            }
            else if (status == NO_ERROR_STATUS)
            {
                long version = transport.readLong();
                if (logger.IsTraceEnabled)
                    logger.Trace("Received version: " + version);

                byte[] value = transport.readArray();
                if (logger.IsTraceEnabled)
                    logger.Trace("Received value: " + UTF8Encoding.UTF8.GetString(value));
                result = new BinaryVersionedValue(version, value);
            }
            return result;
        }
    }
}
