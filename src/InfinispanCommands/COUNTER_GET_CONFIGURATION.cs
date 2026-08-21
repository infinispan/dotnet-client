using System;
using System.Text;

namespace Infinispan.Hotrod.Commands
{
    public class COUNTER_GET_CONFIGURATION : Command
    {
        public COUNTER_GET_CONFIGURATION(string name)
        {
            _name = name;
            NetworkReceive = OnReceive;
        }

        private readonly string _name;
        public CounterConfiguration Configuration;

        public override string Name => "COUNTER_GET_CONFIGURATION";
        public override byte Code => 0x4D;

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            Codec.writeArray(Encoding.UTF8.GetBytes(_name), stream);
            stream.Flush();
        }

        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            if (request.ResponseStatus != Codec30.NO_ERROR_STATUS)
                return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Null };
            byte flags = (byte)stream.ReadByte();
            Configuration = CounterConfiguration.Decode(flags, stream);
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
        }
    }
}
