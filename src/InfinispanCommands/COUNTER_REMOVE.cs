using System;
using System.Text;

namespace Infinispan.Hotrod.Commands
{
    public class COUNTER_REMOVE : Command
    {
        public COUNTER_REMOVE(string name)
        {
            _name = name;
            NetworkReceive = OnReceive;
        }

        private readonly string _name;

        public override string Name => "COUNTER_REMOVE";
        public override byte Code => 0x5E;

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            Codec.writeArray(Encoding.UTF8.GetBytes(_name), stream);
            stream.Flush();
        }

        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
        }
    }
}
