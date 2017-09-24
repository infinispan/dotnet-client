using Infinispan.HotRod.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinispan.HotRod
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class CompatibilityMarshaller : IMarshaller
    {
        private const byte VERSION = 3;

        private const byte ID_NULL = 0x01;
        private const byte ID_STRING_EMPTY = 0x3d;
        private const byte ID_STRING_SMALL = 0x3e;
        private const byte ID_STRING_MEDIUM = 0x3f;
        private const byte ID_STRING_LARGE = 0x40;

        public bool IsMarshallable(object o)
        {
            throw new NotImplementedException();
        }

        public object ObjectFromByteBuffer(byte[] buf)
        {
            if (buf[0] != VERSION)
            {
                throw new HotRodClientException(String.Format("Unknown compatibility serialization version: {0}.", buf[0]));
            }
            int startIndex = 2;
            switch (buf[1])
            {
                case ID_NULL:
                    return null;
                case ID_STRING_EMPTY:
                    return "";
                case ID_STRING_SMALL:
                    startIndex += 1;
                    break;
                case ID_STRING_MEDIUM:
                    startIndex += 2;
                    break;
                case ID_STRING_LARGE:
                    startIndex += 4;
                    break;
                default:
                    throw new HotRodClientException(String.Format("Cannot deserialize non-string (type {0}).", buf[1]));
            }
            return System.Text.Encoding.UTF8.GetString(buf, startIndex, buf.Length - startIndex);
        }

        public object ObjectFromByteBuffer(byte[] buf, int offset, int length)
        {
            throw new NotImplementedException();
        }

        public byte[] ObjectToByteBuffer(object obj)
        {
            if ((obj != null) && !(obj is string))
            {
                throw new HotRodClientException(String.Format("Cannot serialize non-string object: {0}.", obj));
            }

            List<byte> bytes = new List<byte>();
            bytes.Add(VERSION);

            if (obj == null)
            {
                bytes.Add(ID_NULL);
            }
            else
            {
                string str = (string)obj;
                int length = str.Length;
                if (length == 0)
                {
                    bytes.Add(ID_STRING_EMPTY);
                }
                else if (length <= 0x100)
                {
                    bytes.Add(ID_STRING_SMALL);
                    bytes.Add((byte)length);
                }
                else if (length <= 0x10000)
                {
                    bytes.Add(ID_STRING_MEDIUM);
                    bytes.Add((byte)(length >> 8));
                    bytes.Add((byte)length);
                }
                else
                {
                    bytes.Add(ID_STRING_LARGE);
                    bytes.Add((byte)(length >> 24));
                    bytes.Add((byte)(length >> 16));
                    bytes.Add((byte)(length >> 8));
                    bytes.Add((byte)length);
                }
                if (length != 0)
                {
                    bytes.AddRange(System.Text.Encoding.UTF8.GetBytes(str));
                }
            }
            return bytes.ToArray();
        }

        public byte[] ObjectToByteBuffer(object obj, int estimatedSize)
        {
            throw new NotImplementedException();
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
