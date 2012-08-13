using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient.Trans;
using NLog;
using Infinispan.DotnetClient;


namespace Infinispan.DotNetClient.Operations
{
    /*
    * Removes entries from cache if it isunmodified
    *  
    * Author: sunimalr@gmail.com
    * 
    */
    public class RemoveIfUnmodifiedOperation : AbstractKeyOperation<VersionedOperationResponse>
    {
        private readonly long version;
        private Logger logger;
        public RemoveIfUnmodifiedOperation(Codec codec, byte[] key, byte[] cacheName, int topologyId, Flag[] flags, long version) :
            base(codec, key, cacheName, topologyId, flags)
        {
            this.version = version;
            logger = LogManager.GetLogger("RemoveIfUnmodifiedOperation");
        }

        public VersionedOperationResponse executeOperation(ITransport transport)
        {
            // 1) write header
            HeaderParams param = writeHeader(transport, REMOVE_IF_UNMODIFIED_REQUEST);

            //2) write message body
            transport.writeArray(key);
            transport.writeLong(version);
            logger.Trace("written : key = " + key + " version = " + version); 
            transport.flush();
            //process response and return
            return returnVersionedOperationResponse(transport, param);
        }
    }
}
