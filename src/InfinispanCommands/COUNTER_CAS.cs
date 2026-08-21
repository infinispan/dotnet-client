using System;
using System.Text;

namespace Infinispan.Hotrod.Commands
{
    public class COUNTER_CAS : Command
    {
        public COUNTER_CAS(string name, long expect, long update)
        {
            _name = name;
            _expect = expect;
            _update = update;
            NetworkReceive = OnReceive;
        }

        private readonly string _name;
        private readonly long _expect;
        private readonly long _update;
        public long OldValue;
        public bool Success;

        public override string Name => "COUNTER_CAS";
        public override byte Code => 0x58;

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            Codec.writeArray(Encoding.UTF8.GetBytes(_name), stream);
            Codec.writeLong(_expect, stream);
            Codec.writeLong(_update, stream);
            stream.Flush();
        }

        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            if (request.ResponseStatus != Codec30.NO_ERROR_STATUS)
                return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Null };
            OldValue = Codec.readLong(stream);
            Success = OldValue == _expect;
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
        }
    }
}
