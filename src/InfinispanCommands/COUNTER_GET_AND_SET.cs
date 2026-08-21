using System;
using System.Text;

namespace Infinispan.Hotrod.Commands
{
    public class COUNTER_GET_AND_SET : Command
    {
        public COUNTER_GET_AND_SET(string name, long value)
        {
            _name = name;
            _value = value;
            NetworkReceive = OnReceive;
        }

        private readonly string _name;
        private readonly long _value;
        public long PreviousValue;

        public override string Name => "COUNTER_GET_AND_SET";
        public override byte Code => 0x7F;

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            Codec.writeArray(Encoding.UTF8.GetBytes(_name), stream);
            Codec.writeLong(_value, stream);
            stream.Flush();
        }

        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            if (request.ResponseStatus != Codec30.NO_ERROR_STATUS)
                return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Null };
            PreviousValue = Codec.readLong(stream);
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
        }
    }
}
