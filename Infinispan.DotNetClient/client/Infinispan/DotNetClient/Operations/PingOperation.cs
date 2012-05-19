using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Infinispan.DotNetClient.Trans;

using Infinispan.DotNetClient.Protocol;
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
        private readonly Transport transport;
        private static Logger logger;

        public PingOperation(Codec codec, int topologyId, Transport trans) :
            this(codec, topologyId, trans, HotRodConstants.DEFAULT_CACHE_NAME_BYTES)
        {
        }

        public PingOperation(Codec codec, int topologyId, Transport transport, byte[] cacheName) :
            base(codec, null, cacheName, topologyId)
        {
            logger = LogManager.GetLogger("PingOperation");
            this.transport = transport;
        }


        public PingResult execute()
        {
            try
            {
                HeaderParams param = writeHeader(transport, HotRodConstants.PING_REQUEST);
                transport.getBinaryWriter().Flush();
                short respStatus = readHeaderAndValidate(transport, param);
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

        public enum PingResult
        {
            // Success if the ping request was responded correctly
            SUCCESS,
            // When the ping request fails due to non-existing cache
            CACHE_DOES_NOT_EXIST,
            // For any other type of failures
            FAIL,
        }
    }
}
