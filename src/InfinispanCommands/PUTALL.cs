using System;
using System.Collections.Generic;
using System.Text;


namespace Infinispan.Hotrod.Commands
{
    public class PUTALL<K, V> : Command, ICommandWithExpiration
    {
        public PUTALL(Marshaller<K> km, Marshaller<V> vm, IDictionary<K, V> map)
        {
            Map = map;
            KeyMarshaller = km;
            ValueMarshaller = vm;

            NetworkReceive = OnReceive;
        }
        public Marshaller<K> KeyMarshaller;
        public Marshaller<V> ValueMarshaller;
        public int Segment = -1;
        public int TimeOut { get; set; }

        public ExpirationTime Lifespan { get; set; } = new ExpirationTime { Unit = TimeUnit.DEFAULT, Value = 0 };
        public ExpirationTime MaxIdle { get; set; } = new ExpirationTime { Unit = TimeUnit.DEFAULT, Value = 0 };
        public override string Name => "PUTALL";

        public override Byte Code => 0x2D;

        public IDictionary<K, V> Map { get; set; }
        public V PrevValue { get; set; }

        internal override void OnExecute(CommandContext ctx)
        {
            base.OnExecute(ctx);
        }

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            Codec.writeExpirations(Lifespan, MaxIdle, stream);
            Codec.writeVInt(Map.Count, stream);
            foreach (var entry in Map)
            {
                Codec.writeArray(KeyMarshaller.Marshall(entry.Key), stream);
                Codec.writeArray(ValueMarshaller.Marshall(entry.Value), stream);
            }
            stream.Flush();
        }
        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Null };
        }
    }
}
