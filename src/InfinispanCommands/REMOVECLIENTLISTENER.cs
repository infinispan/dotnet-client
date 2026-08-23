using System;
using System.Collections.Generic;
using System.Text;

namespace Infinispan.Hotrod.Commands
{
    public class REMOVECLIENTLISTENER : Command
    {
        public REMOVECLIENTLISTENER(IClientListener listener)
        {
            NetworkReceive = OnReceive;
            this.Listener = listener;
        }
        public override string Name => "REMOVECLIENTLISTENER";

        public override Byte Code => 0x27;
        public Byte ResponseCode => 0x28;

        internal override void OnExecute(CommandContext ctx)
        {
            base.OnExecute(ctx);
        }
        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            base.Execute(ctx, client, stream);
            Codec.writeArray(StringMarshaller._ASCII.Marshall(this.Listener.ListenerID), stream);
            stream.Flush();
        }

        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            if (request.ResponseStatus == Codec30.NO_ERROR_STATUS)
            {
                request.Client.Host.Listeners.TryRemove(this.Listener.ListenerID, out _);
                if (this.Listener is AbstractClientListener acl)
                {
                    acl.Complete();
                }
                return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
            }
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Error };
        }
    }
}
