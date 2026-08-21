using System;
using System.Text;

namespace Infinispan.Hotrod.Commands
{
    public class ITERATION_START : Command
    {
        public int BatchSize = 1000;
        public bool IncludeMetadata;
        public string IterationId;

        public ITERATION_START()
        {
            NetworkReceive = OnReceive;
        }

        public override string Name => "ITERATION_START";
        public override byte Code => 0x31;

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            // Segments: -1 = all segments
            Codec.writeSignedVInt(-1, stream);
            // Filter/converter factory: -1 = none
            Codec.writeSignedVInt(-1, stream);
            // Batch size
            Codec.writeVInt(BatchSize, stream);
            // Include metadata flag
            stream.WriteByte(IncludeMetadata ? (byte)1 : (byte)0);
            stream.Flush();
        }

        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            if (Codec30.hasError(request.ResponseStatus))
                return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Error };

            IterationId = Encoding.ASCII.GetString(Codec.readArray(stream));
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
        }
    }
}
