using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Infinispan.Hotrod
{
    public class Codec
    {

        public static Codec30 getCodec(byte version = 30)
        {
            switch (version)
            {
                default:
                    return new Codec30();
            }
        }
        public static Int64 readVLong(ResponseStream stream)
        {
            byte b = 0x80;
            Int64 i = 0;
            // Read b byte while 0x80 bit is 1
            // Compose final value by R-shifting i and pasting 7 bits at time 
            for (int shift = 0; (b & 0x80) != 0; shift += 7)
            {
                b = (byte)stream.ReadByte();
                i |= (Int64)((b & 0x7FL) << shift);
            }
            return i;
        }
        public static Int32 readVInt(ResponseStream stream)
        {
            return (Int32)readVLong(stream);
        }
        public static UInt32 readVUInt(ResponseStream stream)
        {
            return (UInt32)readVLong(stream);
        }
        public static byte[] readArray(ResponseStream stream)
        {
            int size = readVInt(stream);
            return stream.Read(size);
        }
        public static Int16 readShort(ResponseStream stream)
        {
            Int16 val;
            var b = (byte)stream.ReadByte();
            val = (Int16)(b << 8);
            val += (byte)(stream.ReadByte() & 0xff);
            return val;
        }
        public static UInt16 readUnsignedShort(ResponseStream stream)
        {
            UInt16 val;
            var b = (byte)stream.ReadByte();
            val = (UInt16)(b << 8);
            val |= (byte)(stream.ReadByte() & 0xff);
            return val;
        }


        public static Int32 readInt(ResponseStream stream)
        {
            int b1 = stream.ReadByte();
            int b2 = stream.ReadByte();
            int b3 = stream.ReadByte();
            int b4 = stream.ReadByte();
            return (b1 << 24) | (b2 << 16) | (b3 << 8) | b4;
        }
        public static Int64 readLong(ResponseStream stream)
        {
            Int64 val = (long)readInt(stream) << 32;
            val |= readInt(stream) & 0xFFFFFFFFL;
            return val;
        }
        public static void writeVLong(Int64 val, HotRodStream stream)
        {
            while (val > 0x7f)
            {
                byte b = (byte)(val & 0x7fL | 0x80);
                stream.Write(b);
                val >>= 7;
            }
            stream.Write((byte)val);
        }
        public static void writeVInt(Int32 val, HotRodStream stream)
        {
            writeVLong(val, stream);
        }

        public static void writeSignedVInt(Int32 val, HotRodStream stream)
        {
            writeVInt((val << 1) ^ (val >> 31), stream);
        }

        public static void writeVUInt(UInt32 val, HotRodStream stream)
        {
            while (val > 0x7f)
            {
                byte b = (byte)(val & 0x7fL | 0x80);
                stream.Write(b);
                val >>= 7;
            }
            stream.Write((byte)val);
        }

        public static void writeInt(Int32 v, HotRodStream stream)
        {
            // Write Int value on the wire, MSB first
            stream.Write((byte)(v >> 24));
            stream.Write((byte)(v >> 16));
            stream.Write((byte)(v >> 8));
            stream.Write((byte)v);
        }

        public static void writeLong(Int64 v, HotRodStream stream)
        {
            // Write LONG value on the wire, MSB first
            stream.Write((byte)(v >> 56));
            stream.Write((byte)(v >> 48));
            stream.Write((byte)(v >> 40));
            stream.Write((byte)(v >> 32));
            stream.Write((byte)(v >> 24));
            stream.Write((byte)(v >> 16));
            stream.Write((byte)(v >> 8));
            stream.Write((byte)v);
        }

        public static void writeArray(byte[] arr, HotRodStream stream)
        {
            writeVInt((Int32)arr.Length, stream);
            if (arr.Length > 0)
            {
                stream.Write(arr, 0, arr.Length);
            }
        }
        public static void writeMediaType(MediaType mt, HotRodStream stream)
        {
            if (mt == null)
            {
                stream.WriteByte(0x00);
                return;
            }
            switch (mt.InfoType)
            {
                case 0:
                    stream.WriteByte(0x00);
                    break;
                case 1:
                    stream.WriteByte(0x01);
                    writeVInt(mt.PredefinedMediaType, stream);
                    break;
                case 2:
                    stream.WriteByte(0x02);
                    writeArray(mt.CustomMediaType, stream);
                    if (mt.Params != null)
                    {
                        writeVInt((Int32)mt.Params.Count, stream);
                        foreach (var par in mt.Params)
                        {
                            writeArray(par.Item1, stream); // write parameter key
                            writeArray(par.Item2, stream); // and par value
                        }
                    }
                    else
                    {
                        writeVInt(0, stream);
                    }
                    break;
            }
        }

        public static MediaType readMediaType(ResponseStream stream)
        {
            MediaType mt = new MediaType();
            mt.InfoType = (byte)stream.ReadByte();
            switch (mt.InfoType)
            {
                case 0:
                    break;
                case 1:
                    mt.PredefinedMediaType = readVInt(stream);
                    break;
                case 2:
                    mt.CustomMediaType = readArray(stream);
                    var paramsCount = readVInt(stream);
                    if (paramsCount != 0)
                    {
                        mt.Params = new List<Tuple<byte[], byte[]>>();
                        for (var i = 0; i < paramsCount; i++)
                        {
                            var key = readArray(stream);
                            var value = readArray(stream);
                            mt.Params.Add(new Tuple<byte[], byte[]>(key, value));
                        }
                    }
                    break;
            }
            return mt;
        }
        public static byte[] readPreviousValue(ResponseStream stream, byte version)
        {
            if (version >= 40)
            {
                readMetadataFields(stream);
                return readArray(stream);
            }
            return readArray(stream);
        }

        public static void readMetadataFields(ResponseStream stream)
        {
            var flags = (byte)stream.ReadByte();
            if ((flags & 0x01) == 0)
            {
                readLong(stream);  // created
                readVInt(stream);  // lifespan
            }
            if ((flags & 0x02) == 0)
            {
                readLong(stream);  // lastUsed
                readVInt(stream);  // maxIdle
            }
            readLong(stream);  // version
        }

        public static void writeExpirations(ExpirationTime Lifespan, ExpirationTime MaxIdle, HotRodStream stream)
        {
            byte units = (byte)(((int)Lifespan.Unit << 4) + (int)MaxIdle.Unit);
            stream.WriteByte(units);
            if (Lifespan.hasValue())
            {
                writeVLong(Lifespan.Value, stream);
            }
            if (MaxIdle.hasValue())
            {
                writeVLong(MaxIdle.Value, stream);
            }
        }
    }

    public class ReadArraySession
    {
        public ReadArraySession(int size)
        {
            Size = size;
            CompleteSource = new TaskCompletionSource<byte[]>();
        }
        public int Size;
        private TaskCompletionSource<byte[]> CompleteSource;

        // public bool HasEnoughBytes(ResponseStream stream)
        // {
        //     return stream.Length >= this.Size;
        // }

        public bool IsCompleted() { return CompleteSource.Task.IsCompleted; }
        public byte[] Result
        {
            get
            {
                return CompleteSource.Task.Result;
            }
            set
            {
                CompleteSource.SetResult(value);
            }
        }
    }

    public enum TimeUnit : byte
    {
        SECONDS = 0x00,
        MILLISECONDS = 0x01,
        NANOSECONDS = 0x02,
        MICROSECONDS = 0x03,
        MINUTES = 0x04,
        HOURS = 0x05,
        DAYS = 0x06,
        DEFAULT = 0x07,
        INFINITE = 0x08
    }
    public class ExpirationTime
    {
        public TimeUnit Unit { get; set; }
        public Int64 Value { get; set; }
        public bool hasValue()
        {
            return this != null && this.Unit != TimeUnit.DEFAULT && this.Unit != TimeUnit.INFINITE;
        }
    }

    public class MediaType
    {
        public byte InfoType;
        public Int32 PredefinedMediaType;
        public byte[] CustomMediaType;
        public UInt32 ParamsNum;
        public List<Tuple<byte[], byte[]>> Params;
    }

    public class Codec30
    {
        public Codec30() { }

        public static Boolean isSuccess(int status)
        {
            return status == NO_ERROR_STATUS
                || status == NO_ERROR_STATUS_OBJ_STORAGE
                || status == SUCCESS_WITH_PREVIOUS
                || status == SUCCESS_WITH_PREVIOUS_OBJ_STORAGE;
        }
        public static Boolean isNotExecuted(int status)
        {
            return status == NOT_PUT_REMOVED_REPLACED_STATUS
                || status == NOT_EXECUTED_WITH_PREVIOUS
                || status == NOT_EXECUTED_WITH_PREVIOUS_OBJ_STORAGE;
        }

        public static Boolean isNotExist(int status)
        {
            return status == KEY_DOES_NOT_EXIST_STATUS;
        }

        public static Boolean hasPrevious(int status)
        {
            return status == SUCCESS_WITH_PREVIOUS
                    || status == SUCCESS_WITH_PREVIOUS_OBJ_STORAGE
                    || status == NOT_EXECUTED_WITH_PREVIOUS
                    || status == NOT_EXECUTED_WITH_PREVIOUS_OBJ_STORAGE;
        }

        public static Boolean hasError(int status)
        {
            switch (status)
            {
                case INVALID_MAGIC_OR_MESSAGE_ID_STATUS:
                case UNKNOWN_COMMAND_STATUS:
                case UNKNOWN_VERSION_STATUS:
                case REQUEST_PARSING_ERROR_STATUS:
                case SERVER_ERROR_STATUS:
                case COMMAND_TIMEOUT_STATUS:
                    return true;
            }
            return false;
        }
        //response status
        public const byte NO_ERROR_STATUS = 0x00;
        public const byte NOT_PUT_REMOVED_REPLACED_STATUS = 0x01;
        public const byte KEY_DOES_NOT_EXIST_STATUS = 0x02;
        public const byte SUCCESS_WITH_PREVIOUS = 0x03;
        public const byte NOT_EXECUTED_WITH_PREVIOUS = 0x04;
        public const byte INVALID_ITERATION = 0x05;
        public const byte NO_ERROR_STATUS_OBJ_STORAGE = 0x06;
        public const byte SUCCESS_WITH_PREVIOUS_OBJ_STORAGE = 0x07;
        public const byte NOT_EXECUTED_WITH_PREVIOUS_OBJ_STORAGE = 0x08;
        public const byte INVALID_MAGIC_OR_MESSAGE_ID_STATUS = 0x81;
        public const byte REQUEST_PARSING_ERROR_STATUS = 0x84;
        public const byte UNKNOWN_COMMAND_STATUS = 0x82;
        public const byte UNKNOWN_VERSION_STATUS = 0x83;
        public const byte SERVER_ERROR_STATUS = 0x85;
        public const byte COMMAND_TIMEOUT_STATUS = 0x86;
        public const byte NODE_SUSPECTED = 0x87;
        public const byte ILLEGAL_LIFECYCLE_STATE = 0x88;

    }
}

