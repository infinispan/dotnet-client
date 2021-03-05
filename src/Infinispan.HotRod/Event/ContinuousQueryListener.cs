using Google.Protobuf;
using Org.Infinispan.Protostream;
using Org.Infinispan.Query.Remote.Client;
using Infinispan.HotRod.Protobuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 1591
namespace Infinispan.HotRod.Event
{
    public class ContinuousQueryListener<K, V>
    {
        public ContinuousQueryListener(string query)
        {
            this.query = query;
        }
        internal void ContinuousQueryListenerFunction(ClientCacheEntryCustomEvent ev)
        {
            byte[] data = ev.GetEventData();
            try
            {
                WrappedMessage wm = WrappedMessage.Parser.ParseFrom(data);
                ContinuousQueryResult r = ContinuousQueryResult.Parser.ParseFrom(wm.WrappedMessageBytes);
                ContinuousQueryResult.Types.ResultType t = r.ResultType;
                K k = (K)marshaller.ObjectFromByteBuffer(r.Key.ToByteArray());
                V v= default(V);
                if (r.Projection.Count != 0)
                {
                    WrappedMessage[] awm= r.Projection.ToArray();
                    object[] ret = new object[awm.Length];
                    for(var i=0; i<awm.Length; i++)
                    {
                        ret[i] = marshaller.ObjectFromByteBuffer(awm[i].ToByteArray());
                    }
                    v = (V)(object)ret;
                }
                else
                {
                    v = (V)marshaller.ObjectFromByteBuffer(r.Value.ToByteArray());
                }
                switch (t)
                {
                    case ContinuousQueryResult.Types.ResultType.Joining:
                        JoiningCallback(k, v);
                        break;
                    case ContinuousQueryResult.Types.ResultType.Leaving:
                        LeavingCallback(k, v);
                        break;
                    case ContinuousQueryResult.Types.ResultType.Updated:
                        UpdatedCallback(k, v);
                        break;
                    default:
                        break;
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public ClientListener<K, V> clientEventListener;
        public Action<K, V> JoiningCallback { get; set; }
        public Action<K, V> LeavingCallback { get; set; }
        public Action<K, V> UpdatedCallback { get; set; }
        public Action FailoverCallback { get; set; }
        public string query { get; set; }
        public IMarshaller marshaller { get; set; }
}
}
