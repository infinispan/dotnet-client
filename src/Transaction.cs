using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;

namespace Infinispan.Hotrod
{
    public class Xid
    {
        public int FormatId { get; }
        public byte[] GlobalTransactionId { get; }
        public byte[] BranchQualifier { get; }

        public Xid(int formatId, byte[] globalTransactionId, byte[] branchQualifier)
        {
            FormatId = formatId;
            GlobalTransactionId = globalTransactionId;
            BranchQualifier = branchQualifier;
        }

        private static long _counter;

        public static Xid Create()
        {
            var id = Interlocked.Increment(ref _counter);
            var gtid = new byte[16];
            RandomNumberGenerator.Fill(gtid);
            BitConverter.TryWriteBytes(gtid.AsSpan(0, 8), id);
            var bq = new byte[8];
            RandomNumberGenerator.Fill(bq);
            return new Xid(1, gtid, bq);
        }
    }

    [Flags]
    public enum ControlByte : byte
    {
        NOT_READ = 0x1,
        NON_EXISTING = 0x2,
        REMOVE_OP = 0x4
    }

    public class TransactionModification
    {
        public byte[] Key;
        public byte[] Value;
        public long VersionRead;
        public ExpirationTime Lifespan = new ExpirationTime { Unit = TimeUnit.DEFAULT, Value = 0 };
        public ExpirationTime MaxIdle = new ExpirationTime { Unit = TimeUnit.DEFAULT, Value = 0 };
        public byte Control;
    }

    internal class TransactionEntry
    {
        public byte[] Value;
        public long Version;
        public bool Read;
        public bool Existed;
        public bool Removed;
        public ExpirationTime Lifespan;
        public ExpirationTime MaxIdle;
    }

    public static class XaReturnCode
    {
        public const int XA_OK = 0;
        public const int XA_RDONLY = 3;
        public const int XA_HEURRB = 6;
        public const int XA_HEURCOM = 7;
        public const int XA_HEURMIX = 8;
        public const int XA_RBROLLBACK = 100;
    }
}
