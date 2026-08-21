using System;
using System.Collections.Generic;
using System.Text;

namespace Infinispan.Hotrod.Commands
{
    public class ITERATION_NEXT : Command
    {
        private readonly string _iterationId;

        public List<IterationEntry> Entries = new();
        public bool Finished;

        public ITERATION_NEXT(string iterationId)
        {
            _iterationId = iterationId;
            NetworkReceive = OnReceive;
        }

        public override string Name => "ITERATION_NEXT";
        public override byte Code => 0x33;

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            Codec.writeArray(Encoding.ASCII.GetBytes(_iterationId), stream);
            stream.Flush();
        }

        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            if (request.ResponseStatus == Codec30.INVALID_ITERATION)
                return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Error, Messge = "Invalid iteration" };

            // Finished segments (BitSet as byte array) — consumed but not used
            Codec.readArray(stream);

            var entryCount = Codec.readVInt(stream);
            if (entryCount == 0)
            {
                Finished = true;
                return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
            }

            var projectionSize = Codec.readVInt(stream);

            for (int i = 0; i < entryCount; i++)
            {
                var entry = new IterationEntry();
                var metaMarker = (byte)stream.ReadByte();
                if (metaMarker == 1)
                {
                    entry.HasMetadata = true;
                    var flags = (byte)stream.ReadByte();
                    if ((flags & 0x01) == 0)
                    {
                        entry.Created = Codec.readLong(stream);
                        entry.Lifespan = Codec.readVInt(stream);
                    }
                    if ((flags & 0x02) == 0)
                    {
                        entry.LastUsed = Codec.readLong(stream);
                        entry.MaxIdle = Codec.readVInt(stream);
                    }
                    entry.Version = Codec.readLong(stream);
                }

                entry.Key = Codec.readArray(stream);

                if (projectionSize > 1)
                {
                    // Multi-value projection — concatenate for now
                    for (int j = 0; j < projectionSize; j++)
                        Codec.readArray(stream);
                }
                else
                {
                    entry.Value = Codec.readArray(stream);
                }

                Entries.Add(entry);
            }

            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
        }
    }

    public class IterationEntry
    {
        public byte[] Key;
        public byte[] Value;
        public bool HasMetadata;
        public long Created = -1;
        public int Lifespan = -1;
        public long LastUsed = -1;
        public int MaxIdle = -1;
        public long Version;
    }
}
