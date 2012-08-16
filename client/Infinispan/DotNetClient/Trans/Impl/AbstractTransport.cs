using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Protocol;
using System.IO;
using Infinispan.DotNetClient.Trans.Impl.TCP;

namespace Infinispan.DotNetClient.Trans
{
    /*
   * 
   * AbstractTransport class implementes the Transport Interface. 
   *
   * Author: sunimalr@gmail.com
   *      
   */

    public abstract class AbstractTransport : ITransport
    {
        //Reads an array of bytes
        public byte[] ReadArray()
        {
            int responseLength = ReadVInt();
            return ReadByteArray(responseLength);
        }

        //reads a string
        public String ReadString()
        {
            byte[] strContent = ReadArray();
            string readString = UTF8Encoding.UTF8.GetString(strContent);
            return readString;
        }

        //
        public long ReadLong()
        {
            byte[] longBytes = ReadByteArray(8);
            long result = 0;
            foreach (byte longByte in longBytes)
            {
                result <<= 8;
                result ^= (long)longByte & 0xFF;
            }

            return result;
        }

        public void WriteLong(long longValue)
        {
            byte[] b = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                long temp = (long)((ulong)longValue >> (i * 8));
                b[7 - i] = (byte)(temp);
            }
            WriteBytes(b);
        }


        public int ReadUnsignedShort()
        {
            byte[] shortBytes = ReadByteArray(2);
            int result = 0;
            foreach (byte longByte in shortBytes)
            {
                result <<= 8;
                result ^= (int)longByte & 0xFF;
            }
            return result;
        }


        public int Read4ByteInt()
        {
            byte[] b = ReadByteArray(4);
            int value = 0;
            for (int i = 0; i < 4; i++)
            {
                int shift = (4 - 1 - i) * 8;
                value += (b[i] & 0x000000FF) << shift;
            }
            return value;
        }


        public void WriteString(String str)
        {
            if (str.Length > 0)
            {
                WriteArray(UTF8Encoding.UTF8.GetBytes(str));

            }
            else
            {
                WriteVInt(0);
            }
        }

        public void WriteArray(byte[] toAppend)
        {
            WriteVInt(toAppend.Length);
            WriteBytes(toAppend);
        }


        //Following metods are Implemented in TCPTransport Class which Inherits AbstractTransport
        public virtual BinaryWriter GetBinaryWriter()
        {
            throw new NotImplementedException();
        }

        public virtual BinaryReader GetBinaryReader()
        {
            throw new NotImplementedException();
        }

        public virtual void WriteVInt(int vint)
        {
            throw new NotImplementedException();
        }

        public virtual void WriteVLong(long l)
        {
            throw new NotImplementedException();
        }

        public virtual long ReadVLong()
        {
            throw new NotImplementedException();
        }

        public virtual int ReadVInt()
        {
            throw new NotImplementedException();
        }

        public virtual void Flush()
        {
            throw new NotImplementedException();
        }

        public virtual byte ReadByte()
        {
            throw new NotImplementedException();
        }

        public virtual void Release()
        {
            throw new NotImplementedException();
        }

        public virtual byte[] ReadByteArray(int size)
        {
            throw new NotImplementedException();
        }

        public virtual byte[] DumpStream()
        {
            throw new NotImplementedException();
        }
        
        public virtual void WriteByte(byte toWrite)
        {
            throw new NotImplementedException();
        }

        public virtual bool Connect()
        {
            throw new NotImplementedException();
        }

        public virtual void InitializeInputBinaryReader()
        {
            throw new NotImplementedException();
        }

        public virtual void WriteBytes(byte[] toAppend)
        {
            throw new NotImplementedException();
        }


        public virtual System.Net.IPAddress GetIpAddress()
        {
            throw new NotImplementedException();
        }

        public virtual int GetServerPort()
        {
            throw new NotImplementedException();
        }


        public virtual System.Net.IPEndPoint IpEndPoint()
        {
            throw new NotImplementedException();
        }


        public virtual void SetTransportFactory(TCPTransportFactory tf)
        {
            throw new NotImplementedException();
        }

        public virtual TCPTransportFactory GetTransportFactory()
        {
            throw new NotImplementedException();
        }
    }
}
