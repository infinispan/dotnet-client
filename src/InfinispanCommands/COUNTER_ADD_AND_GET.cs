using System;
using System.Text;

namespace Infinispan.Hotrod.Commands
{
    public class COUNTER_ADD_AND_GET : Command
    {
        public COUNTER_ADD_AND_GET(string name, long delta)
        {
            _name = name;
            _delta = delta;
            NetworkReceive = OnReceive;
        }

        private readonly string _name;
        private readonly long _delta;
        public long Value;

        public override string Name => "COUNTER_ADD_AND_GET";
        public override byte Code => 0x52;

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            Codec.writeArray(Encoding.UTF8.GetBytes(_name), stream);
            Codec.writeLong(_delta, stream);
            stream.Flush();
        }

        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            if (request.ResponseStatus == 0x04)
                return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Error, Messge = "Counter bound reached" };
            Value = Codec.readLong(stream);
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
        }
    }
}
