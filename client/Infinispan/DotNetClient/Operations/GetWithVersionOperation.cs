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

    public class GetWithVersionOperation : AbstractKeyOperation<VersionedValueImpl>
    {
        private static Logger logger;
        public GetWithVersionOperation(Codec codec, byte[] key, byte[] cacheName, Flag[] flags) :
            base(codec, key, cacheName,flags)
        {
            logger = LogManager.GetLogger("GetWithVersionOperation");
        }

        public IVersionedValue ExecuteOperation(ITransport transport)
        {
            short status = SendKeyOperation(key, transport, GET_WITH_VERSION, GET_WITH_VERSION_RESPONSE);
            IVersionedValue result = null;
            if (status == KEY_DOES_NOT_EXIST_STATUS)
            {
                result = null;
            }
            else if (status == NO_ERROR_STATUS)
            {
                long version = transport.ReadLong();
                if (logger.IsTraceEnabled)
                    logger.Trace("Received version: " + version);

                byte[] value = transport.ReadArray();
                if (logger.IsTraceEnabled)
                    logger.Trace("Received value: " + transport.GetTransportFactory().GetSerializer().Deserialize(value));
                result = new VersionedValueImpl(version, value,transport.GetTransportFactory().GetSerializer());
            }
            return result;
        }
    }
}
