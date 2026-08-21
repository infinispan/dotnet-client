using System;
using System.Collections.Generic;
using System.Text;


namespace Infinispan.Hotrod.Commands
{
    public class CLEAR : Command
    {
        public CLEAR()
        {
            NetworkReceive = OnReceive;
        }
        public int TimeOut { get; set; }
        public override string Name => "CLEAR";

        public override Byte Code => 0x13;
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
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Null };
        }
    }
}
