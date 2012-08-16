using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Infinispan.DotNetClient.Trans.Impl.TCP;

namespace Infinispan.DotNetClient.Trans
{
    /*
   * 
   * Defines the interface of a Transport which is responsible for TCP transportation of data between client and server
   *
   * Author: sunimalr@gmail.com
   *      
   */

    public interface ITransport
    {

        BinaryWriter GetBinaryWriter();

        BinaryReader GetBinaryReader();

        IPAddress GetIpAddress();

        int GetServerPort();

        void WriteArray(byte[] toAppend);

        void WriteByte(byte toWrite);

        void WriteVInt(int vint);

        void WriteVLong(long l);

        long ReadVLong();

        int ReadVInt();

        void Flush();

        bool Connect();

        byte ReadByte();

        void InitializeInputBinaryReader();

        void Release();

        byte[] ReadArray();

        string ReadString();

        byte[] ReadByteArray(int size);

        long ReadLong();

        void WriteLong(long longValue);

        int ReadUnsignedShort();

        int Read4ByteInt();

        void WriteString(string str);

        byte[] DumpStream();

        void WriteBytes(byte[] toAppend);

        IPEndPoint IpEndPoint();

        void SetTransportFactory(TCPTransportFactory tf);

        TCPTransportFactory GetTransportFactory();
    }
}