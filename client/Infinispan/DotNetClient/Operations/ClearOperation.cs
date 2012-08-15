using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Trans;
using NLog;
using Infinispan.DotnetClient;


namespace Infinispan.DotNetClient.Operations
{
    /*
    * Clears an Infinispan cache
    *  
    * Author: sunimalr@gmail.com
    * 
    */
    public class ClearOperation : HotRodOperation
    {
        private static Logger logger;
        public ClearOperation(Codec codec, byte[] cacheName, int topologyId, Flag[] flags) :
            base(codec, flags, cacheName, topologyId)
        {
            logger = LogManager.GetLogger("ClearOperation");
        }

        public void ExecuteOperation(ITransport transport)
        {
            HeaderParams param = WriteHeader(transport, CLEAR_REQUEST);
            if (logger.IsTraceEnabled)
                logger.Trace("Clear Request Sent");
            transport.flush();
            ReadHeaderAndValidate(transport, param);
        }
    }
}
