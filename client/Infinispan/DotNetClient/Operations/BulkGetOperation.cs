using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Operations;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient.Trans;
using NLog;
using Infinispan.DotnetClient;

namespace Infinispan.DotNetClient.Operations
{
    public class BulkGetOperation : HotRodOperation
    {
        private int entryCount;
        private static Logger logger;

        public BulkGetOperation(Codec codec, byte[] cacheName, int topologyId, Flag[] flags, int entryCount) :
            base(codec, flags, cacheName, topologyId)
        {
            this.entryCount = entryCount;
            logger = LogManager.GetLogger("BulkGetOperation");
        }

        public Dictionary<byte[], byte[]> ExecuteOperation(ITransport transport)
        {
            HeaderParams param = WriteHeader(transport, BULK_GET_REQUEST);
            transport.writeVInt(entryCount);
            transport.flush();
            ReadHeaderAndValidate(transport, param);
            Dictionary<byte[], byte[]> result = new Dictionary<byte[], byte[]>();
            while (transport.readByte() == 1)
            {
                result.Add(transport.readArray(), transport.readArray());
            }
            return result;
        }
    }
}
