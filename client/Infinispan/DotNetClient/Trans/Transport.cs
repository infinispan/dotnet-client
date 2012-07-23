using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Infinispan.DotNetClient.Trans
{
    /*
   * 
   * Defines the interface of a Transport which is responsible for TCP transportation of data between client and server
   *
   * Author: sunimalr@gmail.com
   *      
   */

    public interface Transport
    {

        BinaryWriter getBinaryWriter();

        BinaryReader getBinaryReader();

        void writeArray(byte[] toAppend);

        void writeByte(byte toWrite);

        void writeVInt(int vint);

        void writeVLong(long l);

        long readVLong();

        int readVInt();

        void flush();

        bool connect();

        byte readByte();

        void initializeInputBinaryReader();

        void release();

        byte[] readArray();

        string readString();

        byte[] readByteArray(int size);

        long readLong();

        void writeLong(long longValue);

        int readUnsignedShort();

        int read4ByteInt();

        void writeString(string str);

        byte[] dumpStream();

        void writeBytes(byte[] toAppend);
    }
}
