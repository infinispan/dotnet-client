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

        protected AbstractKeyValueOperation(Codec codec, byte[] key, byte[] cacheName,Flag[] flags, byte[] value, int lifespan, int maxIdle) :
            base(codec, key, cacheName,flags)
        {
            this.value = value;
            this.lifespan = lifespan;
            this.maxIdle = maxIdle;
            logger = LogManager.GetLogger("AbstractKeyValueOperation");
        }

        //[header][key length][key][lifespan][max idle][value length][value]
        protected byte SendOperationRequest(ITransport transport, byte opCode, byte opRespCode)
        {
            // 1) write header
            HeaderParams param = WriteHeader(transport, opCode);
            // 2) write key and value
            transport.WriteArray(key);
            transport.WriteVInt(lifespan);
            transport.WriteVInt(maxIdle);
            transport.WriteArray(value);
            transport.GetBinaryWriter().Flush();
            // 3) now read header
            return ReadHeaderAndValidate(transport, param);
        }
    }
}
