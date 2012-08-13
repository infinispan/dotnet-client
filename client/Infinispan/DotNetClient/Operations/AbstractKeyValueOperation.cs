using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient.Trans;
using NLog;
using Infinispan.DotNetClient.Util;
using Infinispan.DotNetClient;
using Infinispan.DotnetClient;


namespace Infinispan.DotNetClient.Operations
{
    /// <summary>
    ///Abstraction of operations that involves a Key and a value. eg: PutOperation
    ///
    ///Author: sunimalr@gmail.com 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AbstractKeyValueOperation<T> : AbstractKeyOperation<T>
    {
        protected readonly byte[] value;
        protected readonly int lifespan;
        protected readonly int maxIdle;
        private static Logger logger;

        protected AbstractKeyValueOperation(Codec codec, byte[] key, byte[] cacheName, int topologyId, Flag[] flags, byte[] value, int lifespan, int maxIdle) :
            base(codec, key, cacheName, topologyId, flags)
        {
            this.value = value;
            this.lifespan = lifespan;
            this.maxIdle = maxIdle;
            logger = LogManager.GetLogger("AbstractKeyValueOperation");
        }

        //[header][key length][key][lifespan][max idle][value length][value]
        protected byte sendOperationRequest(ITransport transport, byte opCode, byte opRespCode)
        {
            // 1) write header
            HeaderParams param = writeHeader(transport, opCode);
            // 2) write key and value
            transport.writeArray(key);
            transport.writeVInt(lifespan);
            transport.writeVInt(maxIdle);
            transport.writeArray(value);
            transport.getBinaryWriter().Flush();
            // 3) now read header
            return readHeaderAndValidate(transport, param);
        }
    }
}
