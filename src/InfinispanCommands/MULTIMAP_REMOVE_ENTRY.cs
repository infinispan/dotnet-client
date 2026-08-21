using System;

namespace Infinispan.Hotrod.Commands
{
    public class MULTIMAP_REMOVE_ENTRY : Command
    {
        private readonly byte[] _key;
        private readonly byte[] _value;
        private readonly bool _supportsDuplicates;

        public MULTIMAP_REMOVE_ENTRY(byte[] key, byte[] value, bool supportsDuplicates)
        {
            _key = key;
            _value = value;
            _supportsDuplicates = supportsDuplicates;
            NetworkReceive = OnReceive;
        }

        public override string Name => "MULTIMAP_REMOVE_ENTRY";
        public override byte Code => 0x6F;
        public bool Removed;

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            Codec.writeArray(_key, stream);
            Codec.writeExpirations(
                new ExpirationTime { Unit = TimeUnit.INFINITE, Value = 0 },
                new ExpirationTime { Unit = TimeUnit.INFINITE, Value = 0 },
                stream);
            Codec.writeArray(_value, stream);
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
