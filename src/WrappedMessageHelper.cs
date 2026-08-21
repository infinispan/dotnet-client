using System;
using System.Collections.Generic;
using System.Text;

namespace Infinispan.Hotrod
{
    public static class WrappedMessageHelper
    {
        private const int FieldWrappedString = 9;
        private const int FieldWrappedInt32 = 5;
        private const int FieldWrappedInt64 = 3;
        private const int FieldWrappedDescriptorFullName = 16;
        private const int FieldWrappedMessageBytes = 17;

        public static byte[] WrapMessage(byte[] messageBytes, string descriptorFullName)
        {
            var buf = new List<byte>();
            AppendLenDelimited(buf, FieldWrappedDescriptorFullName, Encoding.UTF8.GetBytes(descriptorFullName));
            AppendLenDelimited(buf, FieldWrappedMessageBytes, messageBytes);
            return buf.ToArray();
        }

        public static byte[] WrapString(string s)
        {
            var data = Encoding.UTF8.GetBytes(s);
            var buf = new List<byte>();
            AppendLenDelimited(buf, FieldWrappedString, data);
            return buf.ToArray();
        }

        public static byte[] WrapInt32(int v)
        {
            var buf = new List<byte>();
            AppendVarintField(buf, FieldWrappedInt32, (ulong)v);
            return buf.ToArray();
        }

        public static byte[] WrapInt64(long v)
        {
            var buf = new List<byte>();
            AppendVarintField(buf, FieldWrappedInt64, (ulong)v);
            return buf.ToArray();
        }

        public static string UnwrapString(byte[] data)
        {
            string result = null;
            ScanFields(data, (fieldNumber, wireType, value) =>
            {
                if (fieldNumber == FieldWrappedString && wireType == 2)
                    result = Encoding.UTF8.GetString(value);
            });
            return result;
        }

        public static byte[] UnwrapBytes(byte[] data)
        {
            byte[] result = null;
            ScanFields(data, (fieldNumber, wireType, value) =>
            {
                if (fieldNumber == FieldWrappedMessageBytes && wireType == 2)
                    result = value;
            });
            return result;
        }

        public static CQResult DecodeCQResult(byte[] data)
        {
            var r = new CQResult();
            ScanFields(data, (fieldNumber, wireType, value) =>
            {
                switch (fieldNumber)
                {
                    case 1: // resultType
                        r.ResultType = (CQResultType)DecodeVarint(value);
                        break;
                    case 2: // key
                        r.Key = value;
                        break;
                    case 3: // value
                        r.Value = value;
                        break;
                    case 4: // projection (repeated)
                        r.Projections ??= new List<byte[]>();
                        r.Projections.Add(value);
                        break;
                }
            });
            return r;
        }

        internal static void ScanFields(byte[] data, Action<int, int, byte[]> fn)
        {
            int pos = 0;
            while (pos < data.Length)
            {
                var (tag, n) = DecodeUvarint(data, pos);
                if (n <= 0) return;
                pos += n;

                int fieldNumber = (int)(tag >> 3);
                int wireType = (int)(tag & 0x7);

                switch (wireType)
                {
                    case 0: // varint
                        int start = pos;
                        var (_, vn) = DecodeUvarint(data, pos);
                        if (vn <= 0) return;
                        pos += vn;
                        fn(fieldNumber, wireType, data[start..pos]);
                        break;
                    case 1: // fixed64
                        if (pos + 8 > data.Length) return;
                        fn(fieldNumber, wireType, data[pos..(pos + 8)]);
                        pos += 8;
                        break;
                    case 2: // length-delimited
                        var (length, ln) = DecodeUvarint(data, pos);
                        if (ln <= 0) return;
                        pos += ln;
                        if (pos + (int)length > data.Length) return;
                        fn(fieldNumber, wireType, data[pos..(pos + (int)length)]);
                        pos += (int)length;
                        break;
                    case 5: // fixed32
                        if (pos + 4 > data.Length) return;
                        fn(fieldNumber, wireType, data[pos..(pos + 4)]);
                        pos += 4;
                        break;
                    default:
                        return;
                }
            }
        }

        private static void AppendTag(List<byte> buf, int fieldNumber, int wireType)
        {
            AppendUvarint(buf, (ulong)(fieldNumber << 3 | wireType));
        }

        private static void AppendLenDelimited(List<byte> buf, int fieldNumber, byte[] data)
        {
            AppendTag(buf, fieldNumber, 2);
            AppendUvarint(buf, (ulong)data.Length);
            buf.AddRange(data);
        }

        private static void AppendVarintField(List<byte> buf, int fieldNumber, ulong value)
        {
            AppendTag(buf, fieldNumber, 0);
            AppendUvarint(buf, value);
        }

        private static void AppendUvarint(List<byte> buf, ulong v)
        {
            while (v >= 0x80)
            {
                buf.Add((byte)(v | 0x80));
                v >>= 7;
            }
            buf.Add((byte)v);
        }

        private static (ulong value, int bytesRead) DecodeUvarint(byte[] data, int offset)
        {
            ulong v = 0;
            for (int i = 0; i + offset < data.Length && i < 10; i++)
            {
                byte b = data[offset + i];
                v |= (ulong)(b & 0x7F) << (7 * i);
                if (b < 0x80)
                    return (v, i + 1);
            }
            return (0, -1);
        }

        private static ulong DecodeVarint(byte[] data)
        {
            var (v, _) = DecodeUvarint(data, 0);
            return v;
        }
    }
}
