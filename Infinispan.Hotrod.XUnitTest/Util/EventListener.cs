using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infinispan.Hotrod.Tests.Util
{
    class LoggingEventListener : AbstractClientListener
    {
        private Cache<string, string> cache;
        public TaskCompletionSource<bool> serverIsRunning = new TaskCompletionSource<bool>();
        public TaskCompletionSource<bool> ListenerIsAdded = new TaskCompletionSource<bool>();
        public override void OnEvent(Event e)
        {
            if (e.CustomMarker != 0)
            {
                CustomEventAction(e);
                return;
            }
            switch (e.Type)
            {
                case EventType.CREATED:
                    CreatedEventAction(e);
                    break;
                case EventType.MODIFIED:
                    ModifiedEventAction(e);
                    break;
                case EventType.REMOVED:
                    RemovedEventAction(e);
                    break;
                case EventType.EXPIRED:
                    ExpiredEventAction(e);
                    break;
            }
        }
        public override void OnError(Exception ex = null)
        {
            ErrorAction(ex);
        }
        public BlockingCollection<Event> createdEvents = new BlockingCollection<Event>();
        public BlockingCollection<Event> removedEvents = new BlockingCollection<Event>();
        public BlockingCollection<Event> modifiedEvents = new BlockingCollection<Event>();
        public BlockingCollection<Event> expiredEvents = new BlockingCollection<Event>();
        public BlockingCollection<Event> customEvents = new BlockingCollection<Event>();
        public BlockingCollection<Object> ErrorEvents = new BlockingCollection<Object>();

        public override string ListenerID { get; set; }

        public LoggingEventListener(Cache<string, string> cache, String listenerId = default)
        {
            this.cache = cache;
            this.ListenerID = listenerId;
        }

        public void CreatedEventAction(Event e)
        {
            createdEvents.Add(e);
        }

        public void RemovedEventAction(Event e)
        {
            removedEvents.Add(e);
        }

        public void ModifiedEventAction(Event e)
        {
            modifiedEvents.Add(e);
        }

        public void ExpiredEventAction(Event e)
        {
            expiredEvents.Add(e);
        }

        public void CustomEventAction(Event e)
        {
            customEvents.Add(e);
        }
        public void ErrorAction(Exception ex)
        {
            ErrorEvents.Add(ex);
            serverIsRunning.Task.Wait();
            {
                cache.AddListener(this).Wait();
                ListenerIsAdded.TrySetResult(true);
            }
        }

        public void CleanErrors()
        {
            ErrorEvents = new BlockingCollection<object>();
        }
        public Event PollEvent(EventType eventType, byte isCustom = 0)
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            tokenSource.CancelAfter(20000);
            try
            {
                if (isCustom != 0)
                {
                    return customEvents.Take(token);
                }
                switch (eventType)
                {
                    case EventType.CREATED:
                        return createdEvents.Take(token);
                    case EventType.MODIFIED:
                        return modifiedEvents.Take(token);
                    case EventType.REMOVED:
                        return removedEvents.Take(token);
                    case EventType.EXPIRED:
                        return expiredEvents.Take(token);
                    default:
                        throw new ArgumentException("Uknown event type");
                }
            }
            catch (OperationCanceledException ex)
            {
                string s = "cre: " + createdEvents.Count + "  mod: " + modifiedEvents.Count + "  rem: " + removedEvents.Count + " exp: " + expiredEvents.Count + " err:" + ErrorEvents.Count;
                throw new TimeoutException("The event of type " + eventType.ToString() + " was not received within timeout!  " + s, ex);
            }
        }
    }
}
