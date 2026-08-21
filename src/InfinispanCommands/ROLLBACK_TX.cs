using System;

namespace Infinispan.Hotrod.Commands
{
    public class ROLLBACK_TX : Command
    {
        private readonly Xid _xid;

        public int XaReturnCode { get; private set; }

        public ROLLBACK_TX(Xid xid)
        {
            _xid = xid;
            NetworkReceive = OnReceive;
        }

        public override string Name => "ROLLBACK_TX";
        public override byte Code => 0x3F;

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            Codec.writeSignedVInt(_xid.FormatId, stream);
            Codec.writeArray(_xid.GlobalTransactionId, stream);
            Codec.writeArray(_xid.BranchQualifier, stream);
            stream.Flush();
        }

        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            if (Codec30.isSuccess(request.ResponseStatus))
            {
                XaReturnCode = Codec.readInt(stream);
                return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
            }
            XaReturnCode = Hotrod.XaReturnCode.XA_HEURRB;
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
        }
    }
}
