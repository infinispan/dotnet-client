using System;
using System.Collections.Generic;
using System.Text;


namespace Infinispan.Hotrod.Commands
{
    public class SIZE : Command
    {
        public SIZE()
        {
            NetworkReceive = OnReceive;
        }
        public override string Name => "SIZE";

        public override Byte Code => 0x29;
        public Int32 Size;

        internal override void OnExecute(CommandContext ctx)
        {
            base.OnExecute(ctx);
        }

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
        }

        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            Size = Codec.readVInt(stream);
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
        }
    }
}
