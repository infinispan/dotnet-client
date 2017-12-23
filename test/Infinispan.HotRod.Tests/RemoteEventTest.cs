using Infinispan.HotRod.Config;
using Infinispan.HotRod.Tests.Util;
using NUnit.Framework;

namespace Infinispan.HotRod.Tests.StandaloneXml
{
    [TestFixture]
    [Category("standalone_xml")]
    public class RemoteEventTest
    {
        RemoteCacheManager remoteManager;
        const string ERRORS_KEY_SUFFIX = ".errors";
        const string PROTOBUF_SCRIPT_CACHE_NAME = "___script_cache";
        static IMarshaller marshaller;

        [OneTimeSetUp]
        public void BeforeClass()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222);
            conf.ConnectionTimeout(90000).SocketTimeout(6000);
            marshaller = new JBasicMarshaller();
            conf.Marshaller(marshaller);
            remoteManager = new RemoteCacheManager(conf.Build(), true);
        }

        [Test]
        public void BasicEventsTest()
        {
            LoggingEventListener<string> listener = new LoggingEventListener<string>();
            IRemoteCache<string, string> cache = remoteManager.GetCache<string, string>();
            Event.ClientListener<string, string> cl = new Event.ClientListener<string, string>();
            try
            {
                cache.Clear();
                cl.filterFactoryName = "";
                cl.converterFactoryName = "";
                cl.AddListener(listener.CreatedEventAction);
                cl.AddListener(listener.ModifiedEventAction);
                cl.AddListener(listener.RemovedEventAction);
                cl.AddListener(listener.ExpiredEventAction);
                cl.AddListener(listener.CustomEventAction);
                cache.AddClientListener(cl, new string[] { }, new string[] { }, null);
                AssertNoEvents(listener);
                cache.Put("key1", "value1");
                AssertOnlyCreated("key1", listener);
                cache.Put("key1", "value1bis");
                AssertOnlyModified("key1", listener);
                cache.Remove("key1");
                AssertOnlyRemoved("key1", listener);
                cache.Put("key1", "value1", 100, TimeUnit.MILLISECONDS);
                AssertOnlyCreated("key1", listener);
                TimeUtils.WaitFor(() => { return cache.Get("key1") == null; });
                AssertOnlyExpired("key1", listener);
            }
            finally
            {
                if (cl.listenerId != null)
                {
                    cache.RemoveClientListener(cl);
                }
            }
        }

        [Test]
        public void IncludeCurrentStateEventTest()
        {
            LoggingEventListener<string> listener = new LoggingEventListener<string>();
            IRemoteCache<string, string> cache = remoteManager.GetCache<string, string>();
            Event.ClientListener<string, string> cl = new Event.ClientListener<string, string>();
            try
            {
                cache.Clear();
                cache.Put("key1", "value1");
                cl.filterFactoryName = "";
                cl.converterFactoryName = "";
                cl.includeCurrentState = true;
                cl.AddListener(listener.CreatedEventAction);
                AssertNoEvents(listener);
                cache.AddClientListener(cl, new string[] { }, new string[] { }, null);
                AssertOnlyCreated("key1", listener);
            }
            finally
            {
                if (cl.listenerId != null)
                {
                    cache.RemoveClientListener(cl);
                }
            }
        }

        [Test]
        public void ConditionalEventsTest()
        {
            LoggingEventListener<string> listener = new LoggingEventListener<string>();
            IRemoteCache<string, string> cache = remoteManager.GetCache<string, string>();
            Event.ClientListener<string, string> cl = new Event.ClientListener<string, string>();
            try
            {
                cache.Clear();
                cl.filterFactoryName = "";
                cl.converterFactoryName = "";
                cl.AddListener(listener.CreatedEventAction);
                cl.AddListener(listener.ModifiedEventAction);
                cl.AddListener(listener.RemovedEventAction);
                cl.AddListener(listener.ExpiredEventAction);
                cl.AddListener(listener.CustomEventAction);
                cache.AddClientListener(cl, new string[] { }, new string[] { }, null);
                AssertNoEvents(listener);
                cache.PutIfAbsent("key1", "value1");
                AssertOnlyCreated("key1", listener);
                cache.PutIfAbsent("key1", "value1again");
                AssertNoEvents(listener);
                cache.Replace("key1", "modified");
                AssertOnlyModified("key1", listener);
                cache.ReplaceWithVersion("key1", "modified", 0);
                AssertNoEvents(listener);
                IVersionedValue<string> versioned = cache.GetVersioned("key1");
                //TODO: this needs conversion from long to ulong (is it a bug?)
                cache.ReplaceWithVersion("key1", "modified", versioned.GetVersion());
                AssertOnlyModified("key1", listener);
                cache.RemoveWithVersion("key1", 0);
                AssertNoEvents(listener);
                versioned = cache.GetVersioned("key1");
                cache.RemoveWithVersion("key1", versioned.GetVersion());
                AssertOnlyRemoved("key1", listener);
            }
            finally
            {
                if (cl.listenerId != null)
                {
                    cache.RemoveClientListener(cl);
                }
            }
        }

        [Test]
        public void CustomEventsTest()
        {
            LoggingEventListener<string> listener = new LoggingEventListener<string>();
            IRemoteCache<string, string> cache = remoteManager.GetCache<string, string>();
            Event.ClientListener<string, string> cl = new Event.ClientListener<string, string>();
            try
            {
                cache.Clear();
                cl.filterFactoryName = "";
                cl.converterFactoryName = "";
                cl.converterFactoryName = "to-string-converter-factory";
                cl.AddListener(listener.CreatedEventAction);
                cl.AddListener(listener.ModifiedEventAction);
                cl.AddListener(listener.RemovedEventAction);
                cl.AddListener(listener.ExpiredEventAction);
                cl.AddListener(listener.CustomEventAction);
                cache.AddClientListener(cl, new string[] { }, new string[] { }, null);
                cache.Put("key1", "value1");
                AssertOnlyCustom("custom event: key1 value1", listener);
            }
            finally
            {
                if (cl.listenerId != null)
                {
                    cache.RemoveClientListener(cl);
                }
            }
        }

        [Test]
        public void FilterEventsTest()
        {
            LoggingEventListener<string> listener = new LoggingEventListener<string>();
            IRemoteCache<string, string> cache = remoteManager.GetCache<string, string>();
            Event.ClientListener<string, string> cl = new Event.ClientListener<string, string>();
            try
            {
                cache.Clear();
                cl.filterFactoryName = "string-is-equal-filter-factory";
                cl.converterFactoryName = "";
                cl.AddListener(listener.CreatedEventAction);
                cl.AddListener(listener.ModifiedEventAction);
                cl.AddListener(listener.RemovedEventAction);
                cl.AddListener(listener.ExpiredEventAction);
                cl.AddListener(listener.CustomEventAction);
                cache.AddClientListener(cl, new string[] { "wantedkeyprefix" }, new string[] { }, null);
                AssertNoEvents(listener);
                cache.Put("key1", "value1");
                cache.Put("wantedkeyprefix_key1", "value2");
                //only one received; one is ignored
                AssertOnlyCreated("wantedkeyprefix_key1", listener);
                AssertNoEvents(listener);
                cache.Replace("key1", "modified");
                cache.Replace("wantedkeyprefix_key1", "modified");
                AssertOnlyModified("wantedkeyprefix_key1", listener);
                AssertNoEvents(listener);
                cache.Remove("key1");
                cache.Remove("wantedkeyprefix_key1");
                AssertOnlyRemoved("wantedkeyprefix_key1", listener);
                AssertNoEvents(listener);
            }           
            finally
            {
                if (cl.listenerId != null)
                {
                    cache.RemoveClientListener(cl);
                }
            }
        }

        private void AssertNoEvents(LoggingEventListener<string> listener)
        {
            Assert.AreEqual(0, listener.createdEvents.Count);
            Assert.AreEqual(0, listener.removedEvents.Count);
            Assert.AreEqual(0, listener.modifiedEvents.Count);
            Assert.AreEqual(0, listener.expiredEvents.Count);
            Assert.AreEqual(0, listener.customEvents.Count);
        }

        private void AssertOnlyCreated(string key, LoggingEventListener<string> listener)
        {
            var remoteEvent = listener.PollCreatedEvent();
            Assert.AreEqual(key, remoteEvent.GetKey());
            Assert.AreEqual(0, listener.removedEvents.Count);
            Assert.AreEqual(0, listener.modifiedEvents.Count);
            Assert.AreEqual(0, listener.expiredEvents.Count);
            Assert.AreEqual(0, listener.customEvents.Count);
        }

        private void AssertOnlyModified(string key, LoggingEventListener<string> listener)
        {
            var remoteEvent = listener.PollModifiedEvent();
            Assert.AreEqual(key, remoteEvent.GetKey());
            Assert.AreEqual(0, listener.removedEvents.Count);
            Assert.AreEqual(0, listener.createdEvents.Count);
            Assert.AreEqual(0, listener.expiredEvents.Count);
            Assert.AreEqual(0, listener.customEvents.Count);
        }

        private void AssertOnlyRemoved(string key, LoggingEventListener<string> listener)
        {
            var remoteEvent = listener.PollRemovedEvent();
            Assert.AreEqual(key, remoteEvent.GetKey());
            Assert.AreEqual(0, listener.modifiedEvents.Count);
            Assert.AreEqual(0, listener.createdEvents.Count);
            Assert.AreEqual(0, listener.expiredEvents.Count);
            Assert.AreEqual(0, listener.customEvents.Count);
        }

        private void AssertOnlyExpired(string key, LoggingEventListener<string> listener)
        {
            var remoteEvent = listener.PollExpiredEvent();
            Assert.AreEqual(key, remoteEvent.GetKey());
            Assert.AreEqual(0, listener.modifiedEvents.Count);
            Assert.AreEqual(0, listener.createdEvents.Count);
            Assert.AreEqual(0, listener.removedEvents.Count);
            Assert.AreEqual(0, listener.customEvents.Count);
        }

        private void AssertOnlyCustom(string key, LoggingEventListener<string> listener)
        {
            var remoteEvent = listener.PollCustomEvent();
            Assert.AreEqual(key, marshaller.ObjectFromByteBuffer(remoteEvent.GetEventData()));
            Assert.AreEqual(0, listener.modifiedEvents.Count);
            Assert.AreEqual(0, listener.createdEvents.Count);
            Assert.AreEqual(0, listener.removedEvents.Count);
            Assert.AreEqual(0, listener.expiredEvents.Count);
        }
    }
}
