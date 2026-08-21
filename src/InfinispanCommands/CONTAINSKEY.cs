using System;
using System.Collections.Generic;
using System.Text;


namespace Infinispan.Hotrod.Commands
{
    public class CONTAINSKEY<K> : CommandWithKey<K>
    {
        public CONTAINSKEY(Marshaller<K> km, K key)
        {
            Key = key;
            KeyMarshaller = km;
            NetworkReceive = OnReceive;
        }
        public override string Name => "CONTAINSKEY";

        public override Byte Code => 0x0F;
        public Boolean IsContained;

        internal override void OnExecute(CommandContext ctx)
        {
            base.OnExecute(ctx);
        }

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            Codec.writeArray(KeyMarshaller.marshall(Key), stream);
        }

        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            IsContained = request.ResponseStatus == Codec30.NO_ERROR_STATUS;
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
        }
    }
}
