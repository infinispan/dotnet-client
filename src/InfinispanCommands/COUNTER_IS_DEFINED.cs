using System;
using System.Text;

namespace Infinispan.Hotrod.Commands
{
    public class COUNTER_IS_DEFINED : Command
    {
        public COUNTER_IS_DEFINED(string name)
        {
            _name = name;
            NetworkReceive = OnReceive;
        }

        private readonly string _name;
        public bool IsDefined;

        public override string Name => "COUNTER_IS_DEFINED";
        public override byte Code => 0x4F;

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            Codec.writeArray(Encoding.UTF8.GetBytes(_name), stream);
            stream.Flush();
        }

        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            IsDefined = request.ResponseStatus == Codec30.NO_ERROR_STATUS;
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
        }
    }
}
