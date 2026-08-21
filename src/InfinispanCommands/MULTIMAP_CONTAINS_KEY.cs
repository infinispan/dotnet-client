using System;

namespace Infinispan.Hotrod.Commands
{
    public class MULTIMAP_CONTAINS_KEY : Command
    {
        private readonly byte[] _key;
        private readonly bool _supportsDuplicates;

        public MULTIMAP_CONTAINS_KEY(byte[] key, bool supportsDuplicates)
        {
            _key = key;
            _supportsDuplicates = supportsDuplicates;
            NetworkReceive = OnReceive;
        }

        public override string Name => "MULTIMAP_CONTAINS_KEY";
        public override byte Code => 0x75;
        public bool Result;

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            Codec.writeArray(_key, stream);
            stream.WriteByte(_supportsDuplicates ? (byte)1 : (byte)0);
            stream.Flush();
        }

        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            if (request.ResponseStatus == Codec30.KEY_DOES_NOT_EXIST_STATUS)
            {
                Result = false;
                return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
            }
            Result = stream.ReadByte() == 1;
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
        }
    }
}
