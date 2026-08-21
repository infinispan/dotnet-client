using System;
using System.Collections.Generic;
using System.Text;

using Google.Protobuf;
using Org.Infinispan.Query.Remote.Client;

namespace Infinispan.Hotrod.Commands
{
    public class QUERY : Command
    {
        public QUERY(QueryRequest query)
        {
            Query = query;
            NetworkReceive = OnReceive;
        }
        public QueryRequest Query;
        public Org.Infinispan.Query.Remote.Client.QueryResponse QueryResponse;
        public override string Name => "QUERY";
        public override Byte Code => 0x1f;
        internal override void OnExecute(CommandContext ctx)
        {
            base.OnExecute(ctx);
        }

        internal override void Execute(CommandContext ctx, InfinispanConnection client, HotRodStream stream)
        {
            ctx.IsReqResCommand = true;
            ctx.CmdReqMediaType = new MediaType();
            ctx.CmdReqMediaType.InfoType = 2;
            ctx.CmdReqMediaType.CustomMediaType = Encoding.ASCII.GetBytes("application/x-protostream");
            ctx.CmdResMediaType = ctx.CmdReqMediaType;
            base.Execute(ctx, client, stream);

            Codec.writeArray(Query.ToByteArray(), stream);
            stream.Flush();
        }
        public override Result OnReceive(InfinispanRequest request, ResponseStream stream)
        {
            if (request.ResponseStatus == Codec30.KEY_DOES_NOT_EXIST_STATUS)
            {
                return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Null };
            }
            var buf = Codec.readArray(stream);
            QueryResponse = Org.Infinispan.Query.Remote.Client.QueryResponse.Parser.ParseFrom(buf);
            return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
            // var buf = Codec.readArray(stream, ref size);
            // if (buf != null)
            // {
            //     QueryResponse = Org.Infinispan.Query.Remote.Client.QueryResponse.Parser.ParseFrom(buf);
            //     return new Result { Status = ResultStatus.Completed, ResultType = ResultType.Object };
            // }
            // return new Result { Status = ResultStatus.Loading, ResultType = ResultType.Object };
        }

    }
}
