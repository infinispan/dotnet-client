using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Protocol;
using System.IO;

namespace Infinispan.DotNetClient.Trans
{
    /*
   * 
   * AbstractTransport class implementes the Transport Interface. 
   *
   * Author: sunimalr@gmail.com
   *      
   */

    public abstract class AbstractTransport : Transport
    {
        //Reads an array of bytes
        public byte[] readArray()
        {
            int responseLength = readVInt();
            return readByteArray(responseLength);
        }

        //reads a string
        public String readString()
        {
            byte[] strContent = readArray();
            string readString = UTF8Encoding.UTF8.GetString(strContent);
            return readString;
        }

        //
        public long readLong()
        {
            byte[] longBytes = readByteArray(8);
            long result = 0;
            foreach (byte longByte in longBytes)
            {
                result <<= 8;
                result ^= (long)longByte & 0xFF;
            }

            return result;
        }

        public void writeLong(long longValue)
        {
            byte[] b = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                long temp = (long)((ulong)longValue >> (i * 8));
                b[7 - i] = (byte)(temp);
            }
            writeBytes(b);
        }


        public int readUnsignedShort()
        {
            byte[] shortBytes = readByteArray(2);
            int result = 0;
            foreach (byte longByte in shortBytes)
            {
                result <<= 8;
                result ^= (int)longByte & 0xFF;
            }
            return result;
        }


        public int read4ByteInt()
        {
            byte[] b = readByteArray(4);
            int value = 0;
            for (int i = 0; i < 4; i++)
            {
                int shift = (4 - 1 - i) * 8;
                value += (b[i] & 0x000000FF) << shift;
            }
            return value;
        }


        public void writeString(String str)
        {
            if (str.Length > 0)
            {
                writeArray(UTF8Encoding.UTF8.GetBytes(str));

            }
            else
            {
                writeVInt(0);
            }
        }

        public void writeArray(byte[] toAppend)
        {
            writeVInt(toAppend.Length);
            writeBytes(toAppend);
        }


        //Following metods are Implemented in TCPTransport Class which Inherits AbstractTransport
        public virtual BinaryWriter getBinaryWriter()
        {
            throw new NotImplementedException();
        }

        public virtual BinaryReader getBinaryReader()
        {
            throw new NotImplementedException();
        }

        public virtual void writeVInt(int vint)
        {
            throw new NotImplementedException();
        }

        public virtual void writeVLong(long l)
        {
            throw new NotImplementedException();
        }

        public virtual long readVLong()
        {
            throw new NotImplementedException();
        }

        public virtual int readVInt()
        {
            throw new NotImplementedException();
        }

        public virtual void flush()
        {
            throw new NotImplementedException();
        }

        public virtual byte readByte()
        {
            throw new NotImplementedException();
        }

        public virtual void release()
        {
            throw new NotImplementedException();
        }

        public virtual byte[] readByteArray(int size)
        {
            throw new NotImplementedException();
        }

        public virtual byte[] dumpStream()
        {
            throw new NotImplementedException();
        }
        
        protected virtual void writeBytes(byte[] toAppend) { }


        public virtual void writeByte(byte toWrite)
        {
            throw new NotImplementedException();
        }

        public virtual bool connect()
        {
            throw new NotImplementedException();
        }

        public virtual void initializeInputBinaryReader()
        {
            throw new NotImplementedException();
        }

        void Transport.writeBytes(byte[] toAppend)
        {
            throw new NotImplementedException();
        }


        public System.Net.IPAddress getIpAddress()
        {
            throw new NotImplementedException();
        }

        public int getServerPort()
        {
            throw new NotImplementedException();
        }
    }
}
