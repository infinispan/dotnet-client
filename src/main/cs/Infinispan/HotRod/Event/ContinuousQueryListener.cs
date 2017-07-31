using Google.Protobuf;
using Org.Infinispan.Protostream;
using Org.Infinispan.Query.Remote.Client;
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
        public ContinuousQueryListener(string query,  Func<object, K> keyFromByteBuffer, Func<object,V> valueFromByteBuffer)
        {
            this.query = query;
            this.KeyFromByteBuffer = keyFromByteBuffer;
            this.ValueFromByteBuffer = valueFromByteBuffer;
        }

        internal void ContinuousQueryListenerFunction(ClientCacheEntryCustomEvent ev)
        {
            byte[] data = ev.GetEventData();
            try
            {
                WrappedMessage wm = WrappedMessage.Parser.ParseFrom(data);
                ContinuousQueryResult r = ContinuousQueryResult.Parser.ParseFrom(wm.WrappedMessageBytes);
                ContinuousQueryResult.Types.ResultType t = r.ResultType;
                K k = (K)KeyFromByteBuffer(r.Key.ToByteArray());
                V v= default(V);
                if (r.Projection.Count != 0)
                {
                    WrappedMessage[] awm= r.Projection.ToArray();
                    Collection<byte[]> arrayOfByteArray = new Collection<byte[]>();
                    foreach(WrappedMessage item in awm)
                    {
                        arrayOfByteArray.Add(item.ToByteArray());
                    }
                    v = (V)ValueFromByteBuffer(arrayOfByteArray);
                }
                else
                {
                    WrappedMessage wm1 = WrappedMessage.Parser.ParseFrom(r.Value.ToByteArray());
                    v = (V)ValueFromByteBuffer(wm1.WrappedMessageBytes.ToByteArray());
                }
                switch (t)
                {
                    case ContinuousQueryResult.Types.ResultType.JOINING:
                        JoiningCallback(k, v);
                        break;
                    case ContinuousQueryResult.Types.ResultType.LEAVING:
                        LeavingCallback(k, v);
                        break;
                    case ContinuousQueryResult.Types.ResultType.UPDATED:
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
        public Func<object, K> KeyFromByteBuffer = null;
        public Func<object, V> ValueFromByteBuffer = null;
}
}
