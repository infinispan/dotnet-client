using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient.Trans;
using System.IO;
using NLog;

namespace Infinispan.DotNetClient.Operations
{
    /*
    * To check whether the server is up an ready
    *  
    * Author: sunimalr@gmail.com
    * 
    */
    public class PingOperation : HotRodOperation
    {
        private readonly ITransport transport;
        private static Logger logger;

        public PingOperation(Codec codec, int topologyId, ITransport trans, OperationsFactory opFac) :
            this(codec, topologyId, trans, HotRodConstants.DEFAULT_CACHE_NAME_BYTES,opFac)
        {
        }

        public PingOperation(Codec codec, int topologyId, ITransport transport, byte[] cacheName, OperationsFactory opFac) :
            base(codec, null, cacheName, topologyId,opFac)
        {
            logger = LogManager.GetLogger("PingOperation");
            this.transport = transport;
        }

        public PingResult Execute()
        {
            try
            {
                HeaderParams param = WriteHeader(transport, HotRodConstants.PING_REQUEST);
                transport.GetBinaryWriter().Flush();
                short respStatus = ReadHeaderAndValidate(transport, param);
                if (respStatus == HotRodConstants.NO_ERROR_STATUS)
                {
                    if (logger.IsTraceEnabled)
                        logger.Trace("Successfully validated transport");
                    return PingResult.SUCCESS;
                }
                else
                {
                    if (logger.IsTraceEnabled)
                        logger.Trace("Unknown response status: %s", respStatus);
                    return PingResult.FAIL;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("CacheNotFoundException"))
                    return PingResult.CACHE_DOES_NOT_EXIST;
                else
                    return PingResult.FAIL;
            }
        }
    }
}
