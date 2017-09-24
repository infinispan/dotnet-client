using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 1591
namespace Infinispan.HotRod.Event
{
    public class ClientListener<K, V>
    {
        public bool includeCurrentState = false;
        public string filterFactoryName;
        public string converterFactoryName;
        public bool useRawData = false;
        public char[] listenerId;
        public void AddListener(Action<Event.ClientCacheEntryCreatedEvent<K>> e)
        {
            createdCallbacks.Add(e);
        }
        public void AddListener(Action<Event.ClientCacheEntryModifiedEvent<K>> e)
        {
            modifiedCallbacks.Add(e);
        }
        public void AddListener(Action<Event.ClientCacheEntryRemovedEvent<K>> e)
        {
            removedCallbacks.Add(e);
        }
        public void AddListener(Action<Event.ClientCacheEntryExpiredEvent<K>> e)
        {
            expiredCallbacks.Add(e);
        }
        public void AddListener(Action<Event.ClientCacheEntryCustomEvent> e)
        {
            customCallbacks.Add(e);
        }

        public void ProcessEvent(Event.ClientCacheEntryCreatedEvent<K> e)
        {
            foreach (Action<Event.ClientCacheEntryCreatedEvent<K>> a in createdCallbacks)
            {
                a(e);
            }
        }
        public void ProcessEvent(Event.ClientCacheEntryModifiedEvent<K> e)
        {
            foreach (Action<Event.ClientCacheEntryModifiedEvent<K>> a in modifiedCallbacks)
            {
                a(e);
            }
        }
        public void ProcessEvent(Event.ClientCacheEntryRemovedEvent<K> e)
        {
            foreach (Action<Event.ClientCacheEntryRemovedEvent<K>> a in removedCallbacks)
            {
                a(e);
            }
        }
        public void ProcessEvent(Event.ClientCacheEntryExpiredEvent<K> e)
        {
            foreach (Action<Event.ClientCacheEntryExpiredEvent<K>> a in expiredCallbacks)
            {
                a(e);
            }
        }
        public void ProcessEvent(Event.ClientCacheEntryCustomEvent e)
        {
            foreach (Action<Event.ClientCacheEntryCustomEvent> a in customCallbacks)
            {
                a(e);
            }
        }
        private List<Action<ClientCacheEntryCreatedEvent<K>>> createdCallbacks = new List<Action<ClientCacheEntryCreatedEvent<K>>>();
        private List<Action<ClientCacheEntryModifiedEvent<K>>> modifiedCallbacks = new List<Action<ClientCacheEntryModifiedEvent<K>>>();
        private List<Action<ClientCacheEntryRemovedEvent<K>>> removedCallbacks = new List<Action<ClientCacheEntryRemovedEvent<K>>>();
        private List<Action<ClientCacheEntryExpiredEvent<K>>> expiredCallbacks = new List<Action<ClientCacheEntryExpiredEvent<K>>>();
        private List<Action<ClientCacheEntryCustomEvent>> customCallbacks = new List<Action<ClientCacheEntryCustomEvent>>();

    }
}
