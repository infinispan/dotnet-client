using System;
using System.Collections.Generic;
using System.Text;


namespace Infinispan.Hotrod.Commands
{
    public class REMOVEWITHVERSION<K, V> : CommandWithKey<K>
    {
        public REMOVEWITHVERSION(Marshaller<K> km, Marshaller<V> vm, K key)
        {
            Key = key;
            KeyMarshaller = km;
            ValueMarshaller = vm;

            NetworkReceive = OnReceive;
        }
        public Marshaller<V> ValueMarshaller;
        public int TimeOut { get; set; }
        public Int64 Version;
        public override string Name => "REPLACEWITHVERSION";
        public override Byte Code => 0x0D;
        public V PrevValue { get; private set; }
        public Boolean Removed;
        internal override void OnExecute(CommandContext ctx)
        {
            base.OnExecute(ctx);
        }
        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            Codec.writeArray(KeyMarshaller.marshall(Key), stream);
            Codec.writeLong(Version, stream);
            stream.Flush();
        }
        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            Removed = Codec30.isSuccess(request.ResponseStatus);
            if ((request.Command.Flags & 0x01) == 1 && Codec30.hasPrevious(request.ResponseStatus))
            {
                var retValAsArray = Codec.readPreviousValue(stream, request.context.Version);
                if (retValAsArray.Length > 0)
                {
                    PrevValue = ValueMarshaller.unmarshall(retValAsArray);
                    return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
                }
            }
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Null };
        }
    }
}
