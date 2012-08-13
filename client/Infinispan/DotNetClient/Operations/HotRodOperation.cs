using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient.Trans;
using NLog;
using Infinispan.DotNetClient.Hotrod;

namespace Infinispan.DotNetClient.Operations
{
    /*
    * Parent class of all Hot Rod operations
    *  
    * Author: sunimalr@gmail.com
    * 
    */
    public abstract class HotRodOperation : HotRodConstants
    {
        private static Logger logger;
        protected readonly Flag[] flags;
        protected readonly byte[] cacheName;
        protected readonly int topologyId;
        private readonly Codec codec;
        private static readonly byte NO_TX = 0;
        private static readonly byte XA_TX = 1;

        protected HotRodOperation()
        { }

        protected HotRodOperation(Codec codec, Flag[] flags, byte[] cacheName, int topologyId)
        {
            this.flags = flags;
            this.cacheName = cacheName;
            this.topologyId = topologyId;
            this.codec = codec;
            logger = LogManager.GetLogger("Hot Rod Operation");
        }

        protected HeaderParams writeHeader(Transport transport, byte operationCode)
        {
            HeaderParams param = new HeaderParams().opCode(operationCode).cacheName(cacheName).flags(flags).clientIntel(HotRodConstants.CLIENT_INTELLIGENCE_BASIC).topologyId(topologyId).txMarker(NO_TX);
            if (logger.IsTraceEnabled)
            {
                if (logger.IsTraceEnabled)
                {
                    string msg = ("headerparams created " + operationCode + " " + cacheName + " " + flags + " " + HotRodConstants.CLIENT_INTELLIGENCE_BASIC + " " + topologyId + " " + NO_TX);
                    logger.Trace(msg);
                }
            }
            return codec.writeHeader(transport, param, HotRodConstants.VERSION_11);
        }

        /**
         * Magic	| Message Id | Op code | Status | Topology Change Marker
         */
        protected byte readHeaderAndValidate(Transport transport, HeaderParams param)
        {
            return codec.readHeader(transport, param);
        }
    }
}
