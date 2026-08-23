using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Infinispan.Hotrod
{
    public abstract class Command
    {
        private const int MAX_LENGTH_TABLE = 1024 * 32;

        public static List<byte[]> mMsgHeaderLenData = new List<byte[]>();
        public IClientListener Listener;
        public static byte[] GetMsgHeaderLengthData(int length)
        {
            if (length > MAX_LENGTH_TABLE)
                return null;
            return mMsgHeaderLenData[length - 1];
        }

        public static List<byte[]> mBodyHeaderLenData = new List<byte[]>();

        public static byte[] GetBodyHeaderLenData(int length)
        {
            if (length > MAX_LENGTH_TABLE)
                return null;
            return mBodyHeaderLenData[length - 1];
        }

        public Command(Int32 flags = 0)
        {
            Flags = flags;
        }
        public Func<InfinispanRequest, ResponseStream, Result> NetworkReceive { get; set; }
        public abstract string Name { get; }
        public abstract Byte Code { get; }
        public Int32 Flags { get; set; } // TODO: where to store this?
        internal virtual void OnExecute(CommandContext cache)
        {
        }

        internal virtual void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            OnExecute(ctx); // Build the message. But there's no need to build anything for hotrod
            stream.WriteByte(0xA0);
            Codec.writeVLong(ctx.MessageId, stream);
            stream.Write(ctx.Version);
            stream.Write(Code);
            Codec.writeArray(ctx.CacheNameAsBytes, stream);
            Codec.writeVInt(Flags, stream);
            stream.Write(ctx.ClientIntelligence);
            Codec.writeVUInt(ctx.TopologyId, stream);
            if (ctx.IsReqResCommand)
            {
                Codec.writeMediaType(ctx.CmdReqMediaType, stream);
                Codec.writeMediaType(ctx.CmdResMediaType, stream);
            }
            else
            {
                Codec.writeMediaType(ctx.Cache?.KeyMediaType, stream);
                Codec.writeMediaType(ctx.Cache?.ValueMediaType, stream);
            }
            if (ctx.IsVersion40OrAbove)
            {
                WriteOtherParams(stream);
            }
        }

        protected virtual void WriteOtherParams(HotRodStream stream)
        {
            Codec.writeVInt(0, stream);
        }

        public abstract Result OnReceive(InfinispanRequest request, ResponseStream stream);

        internal enum TopologyKnoledge
        {
            NONE,
            KEY,
            SEGMENT
        }
        internal virtual TopologyKnoledge getTopologyKnowledgeType()
        {
            return TopologyKnoledge.NONE;
        }

        internal virtual byte[] getKeyAsBytes()
        {
            throw new NotImplementedException();
        }

        internal virtual int getSegment()
        {
            throw new NotImplementedException();
        }

    }
    public abstract class CommandWithKey<K> : Command
    {
        public Marshaller<K> KeyMarshaller;
        public K Key { get; set; }
        internal override TopologyKnoledge getTopologyKnowledgeType()
        {
            return TopologyKnoledge.KEY;
        }

        internal override byte[] getKeyAsBytes()
        {
            return KeyMarshaller.Marshall(this.Key);
        }
    }
    public interface ICommandWithExpiration
    {
        public ExpirationTime Lifespan { get; set; }
        public ExpirationTime MaxIdle { get; set; }
    }
}
