using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Infinispan.HotRod.Event;

namespace Infinispan.HotRod.Tests
{
    class LoggingEventListener<E>
    {
        public BlockingCollection<Event.ClientCacheEntryCreatedEvent<E>> createdEvents = new BlockingCollection<Event.ClientCacheEntryCreatedEvent<E>>();
        public BlockingCollection<Event.ClientCacheEntryRemovedEvent<E>> removedEvents = new BlockingCollection<Event.ClientCacheEntryRemovedEvent<E>>();
        public BlockingCollection<Event.ClientCacheEntryModifiedEvent<E>> modifiedEvents = new BlockingCollection<Event.ClientCacheEntryModifiedEvent<E>>();
        public BlockingCollection<Event.ClientCacheEntryExpiredEvent<E>> expiredEvents = new BlockingCollection<Event.ClientCacheEntryExpiredEvent<E>>();
        public BlockingCollection<Event.ClientCacheEntryCustomEvent> customEvents = new BlockingCollection<Event.ClientCacheEntryCustomEvent>();

        public void CreatedEventAction(Event.ClientCacheEntryCreatedEvent<E> e)
        {
            createdEvents.Add(e);
        }

        public void RemovedEventAction(Event.ClientCacheEntryRemovedEvent<E> e)
        {
            removedEvents.Add(e);
        }

        public void ModifiedEventAction(Event.ClientCacheEntryModifiedEvent<E> e)
        {
            modifiedEvents.Add(e);
        }

        public void ExpiredEventAction(Event.ClientCacheEntryExpiredEvent<E> e)
        {
            expiredEvents.Add(e);
        }

        public void CustomEventAction(Event.ClientCacheEntryCustomEvent e)
        {
            customEvents.Add(e);
        }

        public Event.ClientCacheEntryCreatedEvent<E> PollCreatedEvent()
        {
            return PollEvent<Event.ClientCacheEntryCreatedEvent<E>>(typeof(Event.ClientCacheEntryCreatedEvent<E>));
        }

        public Event.ClientCacheEntryRemovedEvent<E> PollRemovedEvent()
        {
            return PollEvent<Event.ClientCacheEntryRemovedEvent<E>>(typeof(Event.ClientCacheEntryRemovedEvent<E>));
        }

        public Event.ClientCacheEntryModifiedEvent<E> PollModifiedEvent()
        {
            return PollEvent<Event.ClientCacheEntryModifiedEvent<E>>(typeof(Event.ClientCacheEntryModifiedEvent<E>));
        }

        public Event.ClientCacheEntryExpiredEvent<E> PollExpiredEvent()
        {
            return PollEvent<Event.ClientCacheEntryExpiredEvent<E>>(typeof(Event.ClientCacheEntryExpiredEvent<E>));
        }

        public Event.ClientCacheEntryCustomEvent PollCustomEvent()
        {
            return PollEvent<Event.ClientCacheEntryCustomEvent>(typeof(Event.ClientCacheEntryCustomEvent));
        }

        private T PollEvent<T>(Type eventType) where T : ClientEvent
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            tokenSource.CancelAfter(10000);
            try
            {
                if (eventType == typeof(Event.ClientCacheEntryCreatedEvent<E>))
                {
                    return createdEvents.Take(token) as T;
                }
                else if (eventType == typeof(Event.ClientCacheEntryRemovedEvent<E>))
                {
                    return removedEvents.Take(token) as T;
                }
                else if (eventType == typeof(Event.ClientCacheEntryModifiedEvent<E>))
                {
                    return modifiedEvents.Take(token) as T;
                }
                else if (eventType == typeof(Event.ClientCacheEntryExpiredEvent<E>))
                {
                    return expiredEvents.Take(token) as T;
                }
                else if (eventType == typeof(Event.ClientCacheEntryCustomEvent))
                {
                    return customEvents.Take(token) as T;
                }
                else
                {
                    throw new ArgumentException("Uknown event type");
                }
            }
            catch (OperationCanceledException ex)
            {
                throw new TimeoutException("The event of type " + eventType.ToString() + " was not received within timeout!", ex);
            }
        }
    }
}
