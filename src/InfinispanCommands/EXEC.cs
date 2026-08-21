using System;
using System.Collections.Generic;
using System.Text;

namespace Infinispan.Hotrod.Commands
{
    public class EXEC : Command
    {
        private readonly string _taskName;
        private readonly List<(string Name, byte[] Value)> _params;

        public EXEC(string taskName, List<(string Name, byte[] Value)> parameters = null)
        {
            _taskName = taskName;
            _params = parameters ?? new List<(string, byte[])>();
            NetworkReceive = OnReceive;
        }

        public override string Name => "EXEC";
        public override byte Code => 0x2B;
        public byte[] Result;

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            Codec.writeArray(Encoding.UTF8.GetBytes(_taskName), stream);
            Codec.writeVInt(_params.Count, stream);
            foreach (var p in _params)
            {
                Codec.writeArray(Encoding.UTF8.GetBytes(p.Name), stream);
                Codec.writeArray(p.Value, stream);
            }
            stream.Flush();
        }

        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            Result = Codec.readArray(stream);
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
        }
    }
}
