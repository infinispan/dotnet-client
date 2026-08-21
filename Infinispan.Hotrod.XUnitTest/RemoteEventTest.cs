using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infinispan.Hotrod.Tests.Util;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Infinispan.Hotrod.XUnitTest
{
    public class RemoteEventTestFixture : IAsyncLifetime
    {
        private InfinispanContainer _container;
        public Cache<string, string> cache;
        public InfinispanClient infinispan = new InfinispanClient();
        public Marshaller<string> marshaller;

        public async Task InitializeAsync()
        {
            _container = new InfinispanContainer("infinispan-noauth.xml");
            await _container.StartAsync();
            infinispan.AddHost(_container.Host, _container.Port);
            infinispan.Version = 0x1f;
            infinispan.ForceReturnValue = false;
            infinispan.ClientIntelligence = 0x01;
            marshaller = new StringMarshaller();
            cache = infinispan.NewCache(marshaller, marshaller, "default");
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }
    }

    [Collection("MainSequence")]
    public class RemoteEventTest : IClassFixture<RemoteEventTestFixture>
    {
        private readonly RemoteEventTestFixture _fixture;
        private Cache<string, string> _cache;
        private InfinispanClient _infinispan;
        private Marshaller<string> _marshaller;
        public RemoteEventTest(RemoteEventTestFixture fixture)
        {
            _fixture = fixture;
            _cache = _fixture.cache;
            _infinispan = _fixture.infinispan;
            _marshaller = _fixture.marshaller;
        }
        const string ERRORS_KEY_SUFFIX = ".errors";
        const string PROTOBUF_SCRIPT_CACHE_NAME = "___script_cache";

        [Fact]
        public async Task BasicEventsTest()
        {
            LoggingEventListener listener = new LoggingEventListener(_cache, "Basic");
            try
            {
                await _cache.Clear();
                await _cache.AddListener(listener);
                AssertNoEvents(listener);
                await _cache.Put("key1", "value1");
                AssertOnly("key1", listener, EventType.CREATED);
                await _cache.Put("key1", "value1bis");
                AssertOnly("key1", listener, EventType.MODIFIED);
                await _cache.Remove("key1");
                AssertOnly("key1", listener, EventType.REMOVED);
                var expire = new ExpirationTime { Unit = TimeUnit.MILLISECONDS, Value = 100 };
                await _cache.Put("key1", "value1", expire);
                AssertOnly("key1", listener, EventType.CREATED);
                TimeUtils.WaitFor(() => { return _cache.Get("key1").Result == null; });
                AssertOnly("key1", listener, EventType.EXPIRED);
            }
            catch (Exception)
            {
            }
            finally
            {
                AssertErrorCount(listener, 0);
                await _cache.RemoveListener(listener);
                AssertErrorCount(listener, 0);
            }
        }

        [Fact]
        public async Task IncludeCurrentStateEventTest()
        {
            LoggingEventListener listener = new LoggingEventListener(_cache, "IncludeCurrentState");
            try
            {
                await _cache.Clear();
                await _cache.Put("key1", "value1");
                AssertNoEvents(listener);
                await _cache.AddListener(listener, true);
                AssertOnly("key1", listener, EventType.CREATED);
            }
            finally
            {
                AssertErrorCount(listener, 0);
                await _cache.RemoveListener(listener);
                AssertErrorCount(listener, 0);
            }
        }

        [Fact]
        public async Task ConditionalEventsTest()
        {
            LoggingEventListener listener = new LoggingEventListener(_cache, "Conditional");
            try
            {
                await _cache.Clear();
                await _cache.AddListener(listener);
                AssertNoEvents(listener);
                await _cache.PutIfAbsent("key1", "value1");
                AssertOnly("key1", listener, EventType.CREATED);
                await _cache.PutIfAbsent("key1", "value1again");
                AssertNoEvents(listener);
                await _cache.Replace("key1", "modified");
                AssertOnly("key1", listener, EventType.MODIFIED);
                await _cache.ReplaceWithVersion("key1", "modified", 0);
                AssertNoEvents(listener);
                ValueWithVersion<string> versioned = await _cache.GetWithVersion("key1");
                await _cache.ReplaceWithVersion("key1", "modified", versioned.Version);
                AssertOnly("key1", listener, EventType.MODIFIED);
                await _cache.RemoveWithVersion("key1", 0);
                AssertNoEvents(listener);
                versioned = await _cache.GetWithVersion("key1");
                await _cache.RemoveWithVersion("key1", versioned.Version);
                AssertOnly("key1", listener, EventType.REMOVED);
            }
            finally
            {
                AssertErrorCount(listener, 0);
                await _cache.RemoveListener(listener);
                AssertErrorCount(listener, 0);
            }
        }
        [Fact(Skip = "Requires server restart capability not available with Testcontainers")]
        public async Task RecoverOnErrorTest()
        {
            await Task.CompletedTask;
        }
        //     [Test]
        //     [Ignore("ISPN-9409")]
        //     public void CustomEventsTest()
        //     {
        //         LoggingEventListener<string> listener = new LoggingEventListener<string>();
        //         IRemoteCache<string, string> cache = remoteManager.GetCache<string, string>();
        //         Event.ClientListener<string, string> cl = new Event.ClientListener<string, string>();
        //         try
        //         {
        //             cache.Clear();
        //             cl.filterFactoryName = "";
        //             cl.converterFactoryName = "";
        //             cl.converterFactoryName = "to-string-converter-factory";
        //             cl.AddListener(listener.CreatedEventAction);
        //             cl.AddListener(listener.ModifiedEventAction);
        //             cl.AddListener(listener.RemovedEventAction);
        //             cl.AddListener(listener.ExpiredEventAction);
        //             cl.AddListener(listener.CustomEventAction);
        //             cache.AddClientListener(cl, new string[] { }, new string[] { }, null);
        //             cache.Put("key1", "value1");
        //             AssertOnlyCustom("custom event: key1 value1", listener);
        //         }
        //         finally
        //         {
        //             if (cl.listenerId != null)
        //             {
        //                 cache.RemoveClientListener(cl);
        //             }
        //         }
        //     }

        //     [Test]
        //     [Ignore("ISPN-9409")]
        //     public void FilterEventsTest()
        //     {
        //         LoggingEventListener<string> listener = new LoggingEventListener<string>();
        //         IRemoteCache<string, string> cache = remoteManager.GetCache<string, string>();
        //         Event.ClientListener<string, string> cl = new Event.ClientListener<string, string>();
        //         try
        //         {
        //             cache.Clear();
        //             cl.filterFactoryName = "string-is-equal-filter-factory";
        //             cl.converterFactoryName = "";
        //             cl.AddListener(listener.CreatedEventAction);
        //             cl.AddListener(listener.ModifiedEventAction);
        //             cl.AddListener(listener.RemovedEventAction);
        //             cl.AddListener(listener.ExpiredEventAction);
        //             cl.AddListener(listener.CustomEventAction);
        //             cache.AddClientListener(cl, new string[] { "wantedkeyprefix" }, new string[] { }, null);
        //             AssertNoEvents(listener);
        //             cache.Put("key1", "value1");
        //             cache.Put("wantedkeyprefix_key1", "value2");
        //             //only one received; one is ignored
        //             AssertOnlyCreated("wantedkeyprefix_key1", listener);
        //             AssertNoEvents(listener);
        //             cache.Replace("key1", "modified");
        //             cache.Replace("wantedkeyprefix_key1", "modified");
        //             AssertOnlyModified("wantedkeyprefix_key1", listener);
        //             AssertNoEvents(listener);
        //             cache.Remove("key1");
        //             cache.Remove("wantedkeyprefix_key1");
        //             AssertOnlyRemoved("wantedkeyprefix_key1", listener);
        //             AssertNoEvents(listener);
        //         }
        //         finally
        //         {
        //             if (cl.listenerId != null)
        //             {
        //                 cache.RemoveClientListener(cl);
        //             }
        //         }
        //     }

        private void AssertNoEvents(LoggingEventListener listener)
        {
            Assert.Empty(listener.createdEvents);
            Assert.Empty(listener.removedEvents);
            Assert.Empty(listener.modifiedEvents);
            Assert.Empty(listener.expiredEvents);
            Assert.Empty(listener.customEvents);
        }

        private void AssertOnly(string key, LoggingEventListener listener, EventType et, bool isCustom = false)
        {
            var remoteEvent = listener.PollEvent(et);
            Assert.Equal(key, _marshaller.unmarshall(remoteEvent.Key));
            if (et != EventType.CREATED || isCustom)
            {
                Assert.Empty(listener.createdEvents);
            }
            if (et != EventType.REMOVED || isCustom)
            {
                Assert.Empty(listener.removedEvents);
            }
            if (et != EventType.MODIFIED || isCustom)
            {
                Assert.Empty(listener.modifiedEvents);
            }
            if (et != EventType.EXPIRED || isCustom)
            {
                Assert.Empty(listener.expiredEvents);
            }
            if (isCustom)
            {
                Assert.Empty(listener.customEvents);
            }
        }
        private void AssertErrorCount(LoggingEventListener listener, int expected)
        {
            Assert.Equal(expected, listener.ErrorEvents.Count);
        }
    }
}
