using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinispan.HotRod.SWIGGen;
using Org.Infinispan.Protostream;

namespace Infinispan.HotRod.Event
{
#pragma warning disable 1591
    class InternalClientEventListenerCallback<K, V> : Infinispan.HotRod.SWIGGen.ClientListenerCallback
    {
        private ClientListener<K, V> cl;
        private IMarshaller marshaller;

        public InternalClientEventListenerCallback(ClientListener<K, V> cl, IMarshaller marshaller)
        {
            this.cl = cl;
            this.marshaller = marshaller;
        }

        public override void processEvent(ClientCacheEventData evData)
        {
            switch (evData.eventType)
            {
                case (byte)EventType.CLIENT_CACHE_ENTRY_CREATED:
                    {
                        ClientCacheEntryCreatedEvent<K> ev = new ClientCacheEntryCreatedEvent<K>((K)unwrap(evData.key), evData.version, evData.isCommandRetried);
                        cl.ProcessEvent(ev);
                    }
                    break;
                case (byte)EventType.CLIENT_CACHE_ENTRY_MODIFIED:
                    {
                        ClientCacheEntryModifiedEvent<K> ev = new ClientCacheEntryModifiedEvent<K>((K)unwrap(evData.key), evData.version, evData.isCommandRetried);
                        cl.ProcessEvent(ev);
                    }
                    break;
                case (byte)EventType.CLIENT_CACHE_ENTRY_REMOVED:
                    {
                        ClientCacheEntryRemovedEvent<K> ev = new ClientCacheEntryRemovedEvent<K>((K)unwrap(evData.key), evData.isCommandRetried);
                        cl.ProcessEvent(ev);
                    }
                    break;
                case (byte)EventType.CLIENT_CACHE_ENTRY_EXPIRED:
                    {
                        ClientCacheEntryExpiredEvent<K> ev = new ClientCacheEntryExpiredEvent<K>((K)unwrap(evData.key));
                        cl.ProcessEvent(ev);
                    }
                    break;
                case (byte)EventType.CLIENT_CACHE_ENTRY_CUSTOM:
                    {
                        byte[] b = evData.data.ToArray();
                        ClientCacheEntryCustomEvent ev = new ClientCacheEntryCustomEvent(b, evData.isCommandRetried);
                        cl.ProcessEvent(ev);
                    }
                    break;
                case (byte)EventType.CLIENT_CACHE_FAILOVER:
                    {
                    }
                    break;
            }
        }
        private Object unwrap(ByteArray input)
        {
            if (input == null)
            {
                return null;
            }
            byte[] barray = new byte[input.getSize()];
            input.copyBytesTo(barray);
            return marshaller.ObjectFromByteBuffer(barray);
        }

        private Object unwrap(VectorChar input)
        {
            if (input == null)
            {
                return null;
            }
            byte[] barray = new byte[input.Count];
            int i = 0;
            foreach (char c in input)
            {
                barray[i++] = (byte)c;
            }
            return marshaller.ObjectFromByteBuffer(barray);
        }

        VectorChar toVectorChar(string s)
        {
            if (s == null) return null;
            VectorChar v = new VectorChar();
            foreach (char c in s)
            {
                v.Add(c);
            }
            return v;
        }

        VectorChar toVectorChar(byte[] s)
        {
            if (s == null) return null;
            VectorChar v = new VectorChar();
            foreach (char c in s)
            {
                v.Add(c);
            }
            return v;
        }

    }
}
