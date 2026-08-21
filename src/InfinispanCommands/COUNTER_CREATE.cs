using System;
using System.Text;

namespace Infinispan.Hotrod.Commands
{
    public class COUNTER_CREATE : Command
    {
        public COUNTER_CREATE(string name, CounterConfiguration config)
        {
            _name = name;
            _config = config;
            NetworkReceive = OnReceive;
        }

        private readonly string _name;
        private readonly CounterConfiguration _config;
        public bool Created;

        public override string Name => "COUNTER_CREATE";
        public override byte Code => 0x4B;

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            Codec.writeArray(Encoding.UTF8.GetBytes(_name), stream);
            stream.Write(_config.EncodeFlags());
            if (_config.Type == CounterType.Weak)
                Codec.writeVInt(_config.ConcurrencyLevel, stream);
            if (_config.Bounded)
            {
                Codec.writeLong(_config.LowerBound, stream);
                Codec.writeLong(_config.UpperBound, stream);
            }
            Codec.writeLong(_config.InitialValue, stream);
            stream.Flush();
        }

        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            Created = request.ResponseStatus == Codec30.NO_ERROR_STATUS;
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
        }
    }
}
