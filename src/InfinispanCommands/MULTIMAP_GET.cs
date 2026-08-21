using System;
using System.Collections.Generic;

namespace Infinispan.Hotrod.Commands
{
    public class MULTIMAP_GET : Command
    {
        private readonly byte[] _key;
        private readonly bool _supportsDuplicates;

        public MULTIMAP_GET(byte[] key, bool supportsDuplicates)
        {
            _key = key;
            _supportsDuplicates = supportsDuplicates;
            NetworkReceive = OnReceive;
        }

        public override string Name => "MULTIMAP_GET";
        public override byte Code => 0x67;
        public List<byte[]> Values;

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            Codec.writeArray(_key, stream);
            stream.WriteByte(_supportsDuplicates ? (byte)1 : (byte)0);
            stream.Flush();
        }

        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            Values = new List<byte[]>();
            if (request.ResponseStatus == Codec30.KEY_DOES_NOT_EXIST_STATUS)
                return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
            var count = Codec.readVInt(stream);
            for (int i = 0; i < count; i++)
                Values.Add(Codec.readArray(stream));
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
        }
    }
}
