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

        public BulkGetOperation(Codec codec, byte[] cacheName, int topologyId, Flag[] flags, int entryCount,OperationsFactory opFac) :
            base(codec, flags, cacheName, topologyId,opFac)
        {
            this.entryCount = entryCount;
            logger = LogManager.GetLogger("BulkGetOperation");
        }

        public Dictionary<byte[], byte[]> ExecuteOperation(ITransport transport)
        {
            HeaderParams param = WriteHeader(transport, BULK_GET_REQUEST);
            transport.WriteVInt(entryCount);
            transport.Flush();
            ReadHeaderAndValidate(transport, param);
            Dictionary<byte[], byte[]> result = new Dictionary<byte[], byte[]>();
            while (transport.ReadByte() == 1)
            {
                result.Add(transport.ReadArray(), transport.ReadArray());
            }
            return result;
        }
    }
}
