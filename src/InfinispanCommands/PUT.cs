using System;
using System.Collections.Generic;
using System.Text;


namespace Infinispan.Hotrod.Commands
{
    public class PUT<K, V> : CommandWithKey<K>, ICommandWithExpiration
    {
        public PUT(Marshaller<K> km, Marshaller<V> vm, K key, V data)
        {
            Key = key;
            Value = data;
            KeyMarshaller = km;
            ValueMarshaller = vm;

            NetworkReceive = OnReceive;
        }
        public Marshaller<V> ValueMarshaller;
        public int TimeOut { get; set; }

        public ExpirationTime Lifespan { get; set; } = new ExpirationTime { Unit = TimeUnit.DEFAULT, Value = 0 };
        public ExpirationTime MaxIdle { get; set; } = new ExpirationTime { Unit = TimeUnit.DEFAULT, Value = 0 };

        public override string Name => "PUT";

        public override Byte Code => 0x01;

        public V Value { get; set; }
        public V PrevValue { get; set; }

        internal override void OnExecute(CommandContext ctx)
        {
            base.OnExecute(ctx);
        }

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            Codec.writeArray(KeyMarshaller.Marshall(Key), stream);
            Codec.writeExpirations(Lifespan, MaxIdle, stream);
            Codec.writeArray(ValueMarshaller.Marshall(Value), stream);
            stream.Flush();
        }
        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            if ((request.Command.Flags & 0x01) == 1 && Codec30.hasPrevious(request.ResponseStatus))
            {
                PrevValue = ValueMarshaller.Unmarshall(Codec.readPreviousValue(stream, request.context.Version));
                return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
            }
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Null };
        }
    }
}
