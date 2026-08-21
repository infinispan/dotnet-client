using System;
using System.Collections.Generic;
using System.Text;


namespace Infinispan.Hotrod.Commands
{
    public class ADDCLIENTLISTENER : Command
    {
        public byte IncludeState;
        private String FilterFactoryName = "";
        private Tuple<byte[], byte[]>[] FilterArgs = null;
        private String ConverterFactoryName = "";
        private Tuple<byte[], byte[]>[] ConverterArgs = null;
        private int Interests = 0;
        private bool isBinary = false;

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
            Codec.writeArray(StringMarshaller._ASCII.marshall(this.Listener.ListenerID), stream);
            stream.WriteByte(this.IncludeState);
            if (!String.IsNullOrEmpty(this.FilterFactoryName))
            {
                Codec.writeArray(Encoding.ASCII.GetBytes(this.FilterFactoryName), stream);
                var argsLen = (byte)(this.FilterArgs == null ? 0 : this.FilterArgs.Length);
                stream.WriteByte(argsLen);
                for (var i = 0; i < argsLen; i++)
                {
                    Codec.writeArray(this.FilterArgs[i].Item1, stream);
                    Codec.writeArray(this.FilterArgs[i].Item2, stream);
                }
            }
            else
            {
                stream.WriteByte(0);
            }
            if (!String.IsNullOrEmpty(this.ConverterFactoryName))
            {
                Codec.writeArray(Encoding.ASCII.GetBytes(this.ConverterFactoryName), stream);
                var argsLen = (byte)(this.ConverterArgs == null ? 0 : this.ConverterArgs.Length);
                stream.WriteByte(argsLen);
                for (var i = 0; i < argsLen; i++)
                {
                    Codec.writeArray(this.ConverterArgs[i].Item1, stream);
                    Codec.writeArray(this.ConverterArgs[i].Item2, stream);
                }
            }
            else
            {
                stream.WriteByte(0);
            }
            Codec.writeVInt(this.Interests, stream);
            stream.WriteByte(isBinary ? (byte)1 : (byte)0);
            stream.Flush();
            // Pre-register so include-state events arriving before the response are dispatched
            client.Host.Listeners[this.Listener.ListenerID] = this.Listener;
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
