using System;
using System.Collections.Generic;
using System.Text;


namespace Infinispan.Hotrod.Commands
{
    public class GETALL<K, V> : Command
    {
        public GETALL(Marshaller<K> km, Marshaller<V> vm, ISet<K> keys)
        {
            KeyMarshaller = km;
            ValueMarshaller = vm;
            Keys = keys;
            NetworkReceive = OnReceive;
        }
        public Marshaller<K> KeyMarshaller;
        public Marshaller<V> ValueMarshaller;
        public int Segment = -1;
        public ISet<K> Keys { get; set; }

        public IDictionary<K, V> Entries;
        public override string Name => "GETALL";

        public override Byte Code => 0x2F;

        internal override void OnExecute(CommandContext ctx)
        {
            base.OnExecute(ctx);
        }

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            Codec.writeVInt(Keys.Count, stream);
            foreach (var k in Keys)
            {
                Codec.writeArray(KeyMarshaller.Marshall(k), stream);
            }
            stream.Flush();
        }

        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            if (request.ResponseStatus == Codec30.NO_ERROR_STATUS)
            {

                var resCount = Codec.readVInt(stream);
                Entries = new Dictionary<K, V>();
                for (var i = 0; i < resCount; i++)
                {
                    K k = KeyMarshaller.Unmarshall(Codec.readArray(stream));
                    V v = ValueMarshaller.Unmarshall(Codec.readArray(stream));
                    Entries.Add(k, v);
                }
                return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
            }
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Null };
        }
        internal override TopologyKnoledge getTopologyKnowledgeType()
        {
            return TopologyKnoledge.SEGMENT;
        }
        internal override int getSegment()
        {
            return Segment;
        }


    }
}
