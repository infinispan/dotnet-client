using System;
using System.Collections.Generic;
using System.Text;


namespace Infinispan.Hotrod.Commands
{
    public class ADDCLIENTLISTENER : Command
    {
        public byte IncludeState;
        public String FilterFactoryName = "";
        public byte[][] FilterParams = null;
        public String ConverterFactoryName = "";
        public byte[][] ConverterParams = null;
        public int Interests = 0;
        public bool isBinary = false;

        public ADDCLIENTLISTENER()
        {
            NetworkReceive = OnReceive;
        }
        public override string Name => "ADDCLIENTLISTENER";

        public override Byte Code => 0x25;
        public Byte ResponseCode => 0x26;

        internal override void OnExecute(CommandContext ctx)
        {
            base.OnExecute(ctx);
        }
        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            Codec.writeArray(StringMarshaller._ASCII.Marshall(this.Listener.ListenerID), stream);
            stream.WriteByte(this.IncludeState);
            WriteFactory(stream, this.FilterFactoryName, this.FilterParams);
            WriteFactory(stream, this.ConverterFactoryName, this.ConverterParams);
            stream.WriteByte(isBinary ? (byte)1 : (byte)0);
            Codec.writeVInt(this.Interests, stream);
            stream.Flush();
            client.Host.Listeners[this.Listener.ListenerID] = this.Listener;
        }

        private static void WriteFactory(HotRodStream stream, string factoryName, byte[][] factoryParams)
        {
            if (!String.IsNullOrEmpty(factoryName))
            {
                Codec.writeArray(Encoding.ASCII.GetBytes(factoryName), stream);
                var count = (byte)(factoryParams == null ? 0 : factoryParams.Length);
                stream.WriteByte(count);
                for (var i = 0; i < count; i++)
                    Codec.writeArray(factoryParams[i], stream);
            }
            else
            {
                stream.WriteByte(0);
            }
        }

        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            if (request.ResponseStatus == Codec30.NO_ERROR_STATUS)
            {
                if (this.Listener is AbstractClientListener acl)
                {
                    acl.Activate();
                }
                return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
            }
            request.Client.Host.Listeners.TryRemove(this.Listener.ListenerID, out _);
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Error };
        }

    }
}
