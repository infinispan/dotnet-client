using System;
using System.Collections.Generic;
using System.Text;


namespace Infinispan.Hotrod.Commands
{
    public class PING : Command
    {
        public PING()
        {
            NetworkReceive = OnReceive;
        }
        public override string Name => "PING";

        public override Byte Code => 0x17;

        public PingResult Result;

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
            if (request.ResponseStatus == Codec30.NO_ERROR_STATUS)
            {
                this.Result = new PingResult();
                this.Result.KeyType = Codec.readMediaType(stream);
                this.Result.ValueType = Codec.readMediaType(stream);
                this.Result.Version = stream.ReadByte();
                var numOps = Codec.readVInt(stream);
                this.Result.Operations = new int[numOps];
                for (var i = 0; i < numOps; i++)
                {
                    this.Result.Operations[i] = Codec.readShort(stream);
                }
                return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
            }
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Error };
        }
    }
}
