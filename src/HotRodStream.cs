using System.IO;

namespace Infinispan.Hotrod
{
    public class HotRodStream
    {
        private readonly Stream _stream;

        public HotRodStream(Stream stream)
        {
            _stream = stream;
        }

        public void WriteByte(byte b)
        {
            _stream.WriteByte(b);
        }

        public void Write(byte b)
        {
            _stream.WriteByte(b);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        public void Flush()
        {
            _stream.Flush();
        }
    }
}
