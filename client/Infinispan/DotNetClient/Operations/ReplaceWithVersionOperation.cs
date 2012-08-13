using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Protocol;
using NLog;
using Infinispan.DotNetClient.Hotrod;

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

        public ReplaceWithVersionOperation(Codec codec, byte[] key, byte[] cacheName, int topologyId, Flag[] flags, byte[] value, int lifespan, int maxIdle, long version) :
            base(codec, key, cacheName, topologyId, flags, value, lifespan, maxIdle)
        {
            this.version = version;
            logger=LogManager.GetLogger("ReplaceIfUnmodifiedOperation");
        }

       public VersionedOperationResponse executeOperation(Transport transport)
        {
            // 1) write header
            HeaderParams param = writeHeader(transport, REPLACE_IF_UNMODIFIED_REQUEST);
            //2) write message body
            transport.writeArray(key);
            transport.writeVInt(lifespan);
            transport.writeVInt(maxIdle);
            transport.writeLong(version);
            transport.writeArray(value);
            logger.Trace("written : key = " + key + " version = " + version + "value = "+value); 
            transport.flush();
            return returnVersionedOperationResponse(transport, param);
        }
    }
}
