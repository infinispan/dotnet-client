using System;

namespace Infinispan.Hotrod.Commands
{
    public class MULTIMAP_PUT : Command
    {
        private readonly byte[] _key;
        private readonly byte[] _value;
        private readonly bool _supportsDuplicates;

        public MULTIMAP_PUT(byte[] key, byte[] value, bool supportsDuplicates)
        {
            _key = key;
            _value = value;
            _supportsDuplicates = supportsDuplicates;
            NetworkReceive = OnReceive;
        }

        public override string Name => "MULTIMAP_PUT";
        public override byte Code => 0x6B;

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            Codec.writeArray(_key, stream);
            Codec.writeExpirations(
                new ExpirationTime { Unit = TimeUnit.DEFAULT, Value = 0 },
                new ExpirationTime { Unit = TimeUnit.DEFAULT, Value = 0 },
                stream);
            Codec.writeArray(_value, stream);
            stream.WriteByte(_supportsDuplicates ? (byte)1 : (byte)0);
            stream.Flush();
        }

        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Null };
        }
    }
}
