using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Protocol;
using NLog;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Impl;
using Infinispan.DotnetClient;


namespace Infinispan.DotNetClient.Operations
{
    /*
    * Retrieves a value with it's version 
    *  
    * Author: sunimalr@gmail.com
    * 
    */

    public class GetWithVersionOperation : AbstractKeyOperation<VersionedValue>
    {
        private static Logger logger;
        public GetWithVersionOperation(Codec codec, byte[] key, byte[] cacheName, int topologyId, Flag[] flags) :
            base(codec, key, cacheName, topologyId, flags)
        {
            logger = LogManager.GetLogger("GetWithVersionOperation");
        }

        public VersionedValue executeOperation(ITransport transport)
        {
            short status = sendKeyOperation(key, transport, GET_WITH_VERSION, GET_WITH_VERSION_RESPONSE);
            VersionedValue result = null;
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
                    logger.Trace("Received value: " + transport.getTransportFactory().getSerializer().Deserialize(value));
                result = new VersionedValue(version, value,transport.getTransportFactory().getSerializer());
            }
            return result;
        }
    }
}
