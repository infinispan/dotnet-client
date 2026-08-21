using System;
using System.Collections.Generic;

namespace Infinispan.Hotrod.Commands
{
    public class PREPARE_TX : Command
    {
        private readonly Xid _xid;
        private readonly bool _onePhaseCommit;
        private readonly bool _recoverable;
        private readonly long _timeoutMs;
        private readonly List<TransactionModification> _modifications;

        public int XaReturnCode { get; private set; }
        public bool ShouldRetry { get; private set; }

        public PREPARE_TX(Xid xid, bool onePhaseCommit, List<TransactionModification> modifications,
            bool recoverable = false, long timeoutMs = 60000)
        {
            _xid = xid;
            _onePhaseCommit = onePhaseCommit;
            _recoverable = recoverable;
            _timeoutMs = timeoutMs;
            _modifications = modifications;
            NetworkReceive = OnReceive;
        }

        public override string Name => "PREPARE_TX";
        public override byte Code => 0x7D;

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            // XID
            Codec.writeSignedVInt(_xid.FormatId, stream);
            Codec.writeArray(_xid.GlobalTransactionId, stream);
            Codec.writeArray(_xid.BranchQualifier, stream);
            // flags
            stream.Write(_onePhaseCommit ? (byte)1 : (byte)0);
            stream.Write(_recoverable ? (byte)1 : (byte)0);
            Codec.writeLong(_timeoutMs, stream);
            // modifications
            Codec.writeVInt(_modifications.Count, stream);
            foreach (var m in _modifications)
            {
                Codec.writeArray(m.Key, stream);
                stream.Write(m.Control);
                bool notRead = (m.Control & (byte)ControlByte.NOT_READ) != 0;
                bool nonExisting = (m.Control & (byte)ControlByte.NON_EXISTING) != 0;
                bool removeOp = (m.Control & (byte)ControlByte.REMOVE_OP) != 0;
                if (!nonExisting && !notRead)
                {
                    Codec.writeLong(m.VersionRead, stream);
                }
                if (!removeOp)
                {
                    Codec.writeExpirations(m.Lifespan, m.MaxIdle, stream);
                    Codec.writeArray(m.Value, stream);
                }
            }
            stream.Flush();
        }

        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            if (Codec30.isSuccess(request.ResponseStatus))
            {
                XaReturnCode = Codec.readInt(stream);
                return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
            }
            if (request.ResponseStatus == Codec30.NOT_PUT_REMOVED_REPLACED_STATUS)
            {
                ShouldRetry = true;
                XaReturnCode = 0;
                return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
            }
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Error, Messge = "Prepare failed" };
        }
    }
}
