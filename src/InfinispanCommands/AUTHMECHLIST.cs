using System;
using System.Collections.Generic;
using System.Text;


namespace Infinispan.Hotrod.Commands
{
    public class AUTHMECHLIST : Command
    {
        public AUTHMECHLIST()
        {
            NetworkReceive = OnReceive;
        }
        public int TimeOut { get; set; }

        public override string Name => "AUTHMECHLIST";
        public override Byte Code => 0x21;
        public string[] availableMechs { get; set; }
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
            var count = Codec.readVInt(stream);
            availableMechs = new string[count];
            for (int i = 0; i < count; i++)
            {
                availableMechs[i] = Encoding.ASCII.GetString(Codec.readArray(stream));
            }
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
        }
    }
}
