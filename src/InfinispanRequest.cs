using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Infinispan.Hotrod
{
    public class ResponseStream
    {
        private readonly Stream _stream;

        public ResponseStream(Stream stream)
        {
            _stream = stream;
        }

        public byte[] Read(int size)
        {
            byte[] buf = new byte[size];
            int offset = 0;
            while (offset < size)
            {
                int read = _stream.Read(buf, offset, size - offset);
                if (read == 0)
                    throw new IOException("Connection closed by peer");
                offset += read;
            }
            return buf;
        }

        public int ReadByte()
        {
            int b = _stream.ReadByte();
            if (b == -1)
                throw new IOException("Connection closed by peer");
            return b;
        }
    }

    public class InfinispanRequest
    {
        public InfinispanRequest() { }

        internal byte ResponseOpCode;
        public byte ResponseStatus;
        internal CommandContext context;
        internal InfinispanClient Cluster;
        public InfinispanConnection Client;
        public Command Command;
    }

    public class TopologyInfo
    {
        public UInt32 TopologyId;
        public List<Tuple<byte[], UInt16>> servers;
        public InfinispanHost[] hosts;
        public byte HashFuncNum;
        public List<List<Int32>> OwnersPerSegment;
    }
}
