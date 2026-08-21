using System;
using System.Text;

namespace Infinispan.Hotrod.Commands
{
    public class ITERATION_END : Command
    {
        private readonly string _iterationId;

        public ITERATION_END(string iterationId)
        {
            _iterationId = iterationId;
            NetworkReceive = OnReceive;
        }

        public override string Name => "ITERATION_END";
        public override byte Code => 0x35;

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            Codec.writeArray(Encoding.ASCII.GetBytes(_iterationId), stream);
            stream.Flush();
        }

        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
        }
    }
}
