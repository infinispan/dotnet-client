using System;
using System.Collections.Generic;
using System.Text;

namespace Infinispan.Hotrod.Commands
{
    public class COUNTER_GET_NAMES : Command
    {
        public COUNTER_GET_NAMES()
        {
            NetworkReceive = OnReceive;
        }

        public IList<string> CounterNames;

        public override string Name => "COUNTER_GET_NAMES";
        public override byte Code => 0x64;

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            stream.Flush();
        }

        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            int count = Codec.readVInt(stream);
            CounterNames = new List<string>(count);
            for (int i = 0; i < count; i++)
            {
                byte[] nameBytes = Codec.readArray(stream);
                CounterNames.Add(Encoding.UTF8.GetString(nameBytes));
            }
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
        }
    }
}
