using System;
using System.Text;
using Infinispan.HotRod;

namespace Infinispan.HotRod.Tests
{
    class JBasicMarshaller : IMarshaller
    {
        public enum Defs : byte { MARSHALL_VERSION = 0x03, SMALL_STRING = 0x3e, MEDIUM_STRING = 0x3f, INTEGER = 0x4b };
        public bool IsMarshallable(object o)
        {
            throw new NotImplementedException();
        }

        static public object ObjectFromByteBufferHelper(byte[] buf)
        {
            if (buf[0] == (byte)Defs.MARSHALL_VERSION)
            {
                switch (buf[1])
                {
                    case (byte)Defs.SMALL_STRING:
                        return Encoding.UTF8.GetString(buf, 3, buf[2]);
                    case (byte)Defs.MEDIUM_STRING:
                        return Encoding.UTF8.GetString(buf, 4, ((int)buf[2] << 8) + buf[3]);
                    case (byte)Defs.INTEGER:
                        int result = 0;
                        for (int i = 0; i < 4; i++)
                        {
                            result <<= 8;
                            result ^= ((int)buf[i + 2]) & 0xFF;
                        }
                        return result;
                }
            }
            throw new NotImplementedException();
        }

        public object ObjectFromByteBuffer(byte[] buf, int offset, int length)
        {
            throw new NotImplementedException();
        }

        static public byte[] ObjectToByteBufferHelper(object obj)
        {
            if (obj.GetType() == typeof(String))
                return StringToByteBuffer((String)obj);
            if (obj.GetType() == typeof(int))
                return IntToByteBuffer((int)obj);

            throw new NotImplementedException();
        }

        private static byte[] IntToByteBuffer(int num)
        {
            byte[] buf = new byte[6];
            buf[0] = (byte)Defs.MARSHALL_VERSION;
            buf[1] = (byte)Defs.INTEGER;
            for (int i = 0; i < 4; i++)
            {
                buf[5 - i] = (byte)((num >> (8 * i)) & 0xFF);
            }
            return buf;
        }

        private static byte[] StringToByteBuffer(string s)
        {
            if (s.Length <= 0x100)
            {
                return marshallSmall(s);
            }
            else
            {
                return marshallMedium(s);
            }
            throw new NotImplementedException();
        }

        private static byte[] marshallSmall(string s)
        {
            byte[] buf = new byte[s.Length + 3];
            // JBoss preamble
            buf[0] = (byte)Defs.MARSHALL_VERSION;
            buf[1] = (byte)Defs.SMALL_STRING;
            buf[2] = (byte)s.Length;
            Buffer.BlockCopy(Encoding.UTF8.GetBytes(s), 0, buf, 3, s.Length);
            return buf;
        }

        private static byte[] marshallMedium(string s)
        {
            byte[] buf = new byte[s.Length + 4];
            // JBoss preamble
            buf[0] = (byte)Defs.MARSHALL_VERSION;
            buf[1] = (byte)Defs.MEDIUM_STRING;
            buf[2] = (byte)(s.Length >> 8);
            buf[3] = (byte)(s.Length & 0xFF);
            Buffer.BlockCopy(Encoding.UTF8.GetBytes(s), 0, buf, 4, s.Length);
            return buf;
        }




        public byte[] ObjectToByteBuffer(object obj, int estimatedSize)
        {
            throw new NotImplementedException();
        }

        public byte[] ObjectToByteBuffer(object obj)
        {
            return ObjectToByteBufferHelper(obj);
        }

        public object ObjectFromByteBuffer(byte[] buf)
        {
            return ObjectFromByteBufferHelper(buf);
        }
    }
}
