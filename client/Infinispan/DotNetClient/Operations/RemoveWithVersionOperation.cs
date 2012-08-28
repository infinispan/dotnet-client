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
    public class RemoveWithVersionOperation : AbstractKeyOperation<VersionedOperationResponse>
    {
        private readonly long version;
        private Logger logger;
        public RemoveWithVersionOperation(Codec codec, byte[] key, byte[] cacheName,Flag[] flags, long version) :
            base(codec, key, cacheName, flags)
        {
            this.version = version;
            logger = LogManager.GetLogger("RemoveIfUnmodifiedOperation");
        }

        public VersionedOperationResponse ExecuteOperation(ITransport transport)
        {
            // 1) write header
            HeaderParams param = WriteHeader(transport, REMOVE_IF_UNMODIFIED_REQUEST);

            //2) write message body
            transport.WriteArray(key);
            transport.WriteLong(version);
            logger.Trace("written : key = " + key + " version = " + version); 
            transport.Flush();
            //process response and return
            return ReturnVersionedOperationResponse(transport, param);
        }
    }
}
