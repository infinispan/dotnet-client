using System;

namespace Infinispan.Hotrod.Commands
{
    public class MULTIMAP_SIZE : Command
    {
        private readonly bool _supportsDuplicates;

        public MULTIMAP_SIZE(bool supportsDuplicates)
        {
            _supportsDuplicates = supportsDuplicates;
            NetworkReceive = OnReceive;
        }

        public override string Name => "MULTIMAP_SIZE";
        public override byte Code => 0x71;
        public long Value;

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            stream.WriteByte(_supportsDuplicates ? (byte)1 : (byte)0);
            stream.Flush();
        }

        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            Value = Codec.readVLong(stream);
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
        }
    }
}
