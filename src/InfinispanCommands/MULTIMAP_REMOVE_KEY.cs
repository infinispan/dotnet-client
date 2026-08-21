using System;

namespace Infinispan.Hotrod.Commands
{
    public class MULTIMAP_REMOVE_KEY : Command
    {
        private readonly byte[] _key;
        private readonly bool _supportsDuplicates;

        public MULTIMAP_REMOVE_KEY(byte[] key, bool supportsDuplicates)
        {
            _key = key;
            _supportsDuplicates = supportsDuplicates;
            NetworkReceive = OnReceive;
        }

        public override string Name => "MULTIMAP_REMOVE_KEY";
        public override byte Code => 0x6D;
        public bool Removed;

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
                Removed = false;
                return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
            }
            Removed = stream.ReadByte() == 1;
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
        }
    }
}
