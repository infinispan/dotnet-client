using System;
using System.Collections.Generic;
using System.Text;


namespace Infinispan.Hotrod.Commands
{
    public class GETWITHVERSION<K, V> : CommandWithKey<K>
    {
        public GETWITHVERSION(Marshaller<K> km, Marshaller<V> vm, K key)
        {
            Key = key;
            KeyMarshaller = km;
            ValueMarshaller = vm;
            NetworkReceive = OnReceive;
        }
        public Marshaller<V> ValueMarshaller;
        public ValueWithVersion<V> ValueWithVersion { get; set; }
        public override string Name => "GETWITHVERSION";

        public override Byte Code => 0x11;

        internal override void OnExecute(CommandContext ctx)
        {
            base.OnExecute(ctx);
        }

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            Codec.writeArray(KeyMarshaller.marshall(Key), stream);
            stream.Flush();
        }

        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            if (request.ResponseStatus == Codec30.KEY_DOES_NOT_EXIST_STATUS)
            {
                return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Null };
            }
            ValueWithVersion = new ValueWithVersion<V>();
            ValueWithVersion.Version = Codec.readLong(stream);
            ValueWithVersion.Value = ValueMarshaller.unmarshall(Codec.readArray(stream));
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
        }
    }
}
