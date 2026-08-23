using System;
using System.Collections.Generic;
using System.Text;


namespace Infinispan.Hotrod.Commands
{
    public class REMOVE<K, V> : CommandWithKey<K>, ICommandWithExpiration
    {
        public REMOVE(Marshaller<K> km, Marshaller<V> vm, K key)
        {
            Key = key;
            KeyMarshaller = km;
            ValueMarshaller = vm;

            NetworkReceive = OnReceive;
        }
        public Marshaller<V> ValueMarshaller;
        public int TimeOut { get; set; }
        public ExpirationTime Lifespan { get; set; } = new ExpirationTime { Unit = TimeUnit.DEFAULT, Value = 0 };
        public ExpirationTime MaxIdle { get; set; } = new ExpirationTime { Unit = TimeUnit.DEFAULT, Value = 0 };
        public override string Name => "REMOVE";

        public override Byte Code => 0x0B;
        public V PrevValue { get; set; }
        public Boolean Removed;
        internal override void OnExecute(CommandContext ctx)
        {
            base.OnExecute(ctx);
        }

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            Codec.writeArray(KeyMarshaller.Marshall(Key), stream);
        }
        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            Removed = !Codec30.isNotExecuted(request.ResponseStatus);
            if ((request.Command.Flags & 0x01) == 1 && Codec30.hasPrevious(request.ResponseStatus))
            {
                PrevValue = ValueMarshaller.Unmarshall(Codec.readPreviousValue(stream, request.context.Version));
                return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
            }
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Null };
        }
    }
}
