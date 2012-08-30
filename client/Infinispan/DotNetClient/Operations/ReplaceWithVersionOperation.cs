using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Protocol;
using NLog;

namespace Infinispan.DotNetClient.Operations
{
    /*
    * Replaces the value of the specified key if the entry on server is not modified 
    *  
    * Author: sunimalr@gmail.com
    * 
    */
    public class ReplaceWithVersionOperation : AbstractKeyValueOperation<VersionedOperationResponse>
    {
        private readonly long version;
        private static Logger logger;

        public ReplaceWithVersionOperation(Codec codec, byte[] key, byte[] cacheName, Flag[] flags, byte[] value, int lifespan, int maxIdle, long version) :
            base(codec, key, cacheName,flags, value, lifespan, maxIdle)
        {
            this.version = version;
            logger = LogManager.GetLogger("ReplaceIfUnmodifiedOperation");
        }

        public VersionedOperationResponse ExecuteOperation(ITransport transport)
        {
            // 1) write header
            HeaderParams param = WriteHeader(transport, REPLACE_IF_UNMODIFIED_REQUEST);
            //2) write message body
            transport.WriteArray(key);
            transport.WriteVInt(lifespan);
            transport.WriteVInt(maxIdle);
            transport.WriteLong(version);
            transport.WriteArray(value);
            if (logger.IsTraceEnabled)
                logger.Trace("written : key = " + key + " version = " + version + "value = " + value);
            transport.Flush();
            return ReturnVersionedOperationResponse(transport, param);
        }
    }
}
