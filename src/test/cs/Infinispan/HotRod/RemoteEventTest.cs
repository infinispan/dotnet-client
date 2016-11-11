using Infinispan.HotRod.Config;
using System.Collections.Generic;
using Infinispan.HotRod.Tests.Util;
using NUnit.Framework;

namespace Infinispan.HotRod.Tests
{
    class RemoteEventTest
    {
        RemoteCacheManager remoteManager;
        const string ERRORS_KEY_SUFFIX = ".errors";
        const string PROTOBUF_SCRIPT_CACHE_NAME = "___script_cache";
        IMarshaller marshaller;

        [TestFixtureSetUp]
        public void BeforeClass()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222);
            conf.ConnectionTimeout(90000).SocketTimeout(6000);
            marshaller = new JBasicMarshaller();
            conf.Marshaller(marshaller);
            remoteManager = new RemoteCacheManager(conf.Build(), true);
        }

        private static int createdEventCounter;
        private static System.Threading.Semaphore createdSemaphore;
        private static void createdEventAction(Event.ClientCacheEntryCreatedEvent<string> e)
        {
            ++createdEventCounter;
            createdSemaphore.Release();
        }

        private static int removedEventCounter;
        private static System.Threading.Semaphore removedSemaphore;
        private static void removedEventAction(Event.ClientCacheEntryRemovedEvent<string> e)
        {
            ++removedEventCounter;
            removedSemaphore.Release();
        }

        private static int modifiedEventCounter;
        private static System.Threading.Semaphore modifiedSemaphore;
        private static void modifiedEventAction(Event.ClientCacheEntryModifiedEvent<string> e)
        {
            ++modifiedEventCounter;
            modifiedSemaphore.Release();
        }


        [Test]
        public void RemovedEventTest()
        {
            IRemoteCache<string, string> cache = remoteManager.GetCache<string, string>();
            cache.Clear();
            Event.ClientListener<string, string> cl = new Event.ClientListener<string, string>();
            cl.filterFactoryName = "";
            cl.converterFactoryName = "";
            cl.addListener(removedEventAction);
            removedEventCounter = 0;
            removedSemaphore = new System.Threading.Semaphore(0, 1);
            cache.addClientListener(cl, new string[] { }, new string[] { }, null);
            cache.Put("key1", "value1");
            Assert.AreEqual(0, removedEventCounter);
            cache.Remove("key1");
            removedSemaphore.WaitOne();
            Assert.AreEqual(1,removedEventCounter);
            cache.removeClientListener(cl);
        }

        [Test]
        public void CreatedEventTest()
        {
            IRemoteCache<string, string> cache = remoteManager.GetCache<string, string>();
            cache.Clear();
            Event.ClientListener<string, string> cl = new Event.ClientListener<string, string>();
            cl.filterFactoryName = "";
            cl.converterFactoryName = "";
            cl.addListener(createdEventAction);
            createdEventCounter = 0;
            createdSemaphore = new System.Threading.Semaphore(0, 1);
            cache.addClientListener(cl, new string[] { }, new string[] { }, null);
            Assert.AreEqual(0, createdEventCounter);
            cache.Put("key1", "value1");
            createdSemaphore.WaitOne();
            Assert.AreEqual(1, createdEventCounter);
            cache.removeClientListener(cl);
        }

        [Test]
        public void ModifiedEventTest()
        {
            IRemoteCache<string, string> cache = remoteManager.GetCache<string, string>();
            cache.Clear();
            Event.ClientListener<string, string> cl = new Event.ClientListener<string, string>();
            cl.filterFactoryName = "";
            cl.converterFactoryName = "";
            cl.addListener(modifiedEventAction);
            modifiedEventCounter = 0;
            modifiedSemaphore = new System.Threading.Semaphore(0, 1);
            cache.addClientListener(cl, new string[] { }, new string[] { }, null);
            Assert.AreEqual(0, modifiedEventCounter);
            cache.Put("key1", "value1");
            Assert.AreEqual(0, modifiedEventCounter);
            cache.Put("key1", "value1bis");
            modifiedSemaphore.WaitOne();
            Assert.AreEqual(1, modifiedEventCounter);
            cache.removeClientListener(cl);
        }

        [Test]
        public void MultipleEventsTest()
        {
            IRemoteCache<string, string> cache = remoteManager.GetCache<string, string>();
            cache.Clear();
            Event.ClientListener<string, string> cl = new Event.ClientListener<string, string>();
            cl.filterFactoryName = "";
            cl.converterFactoryName = "";
            cl.addListener(modifiedEventAction);
            cl.addListener(createdEventAction);
            cl.addListener(removedEventAction);
            createdEventCounter = 0;
            modifiedEventCounter = 0;
            removedEventCounter = 0;
            createdSemaphore = new System.Threading.Semaphore(0, 1);
            modifiedSemaphore = new System.Threading.Semaphore(0, 1);
            removedSemaphore = new System.Threading.Semaphore(0, 1);
            cache.addClientListener(cl, new string[] { }, new string[] { }, null);
            Assert.AreEqual(0, modifiedEventCounter);
            cache.Put("key1", "value1");
            Assert.AreEqual(0, removedEventCounter);
            Assert.AreEqual(0, modifiedEventCounter);
            createdSemaphore.WaitOne();
            Assert.AreEqual(1, createdEventCounter);
            cache.Put("key1", "value1bis");
            modifiedSemaphore.WaitOne();
            Assert.AreEqual(0, removedEventCounter);
            Assert.AreEqual(1, modifiedEventCounter);
            cache.Remove("key1");
            removedSemaphore.WaitOne();
            Assert.AreEqual(1, removedEventCounter);
            cache.removeClientListener(cl);
        }

        [Test]
        public void IncludeCurrentStateEventTest()
        {
            IRemoteCache<string, string> cache = remoteManager.GetCache<string, string>();
            cache.Clear();
            Event.ClientListener<string, string> cl = new Event.ClientListener<string, string>();
            createdEventCounter = 0;
            cache.Put("key1", "value1");
            cl.filterFactoryName = "";
            cl.converterFactoryName = "";
            cl.includeCurrentState = true;
            Assert.AreEqual(0, createdEventCounter);
            cl.addListener(createdEventAction);
            createdSemaphore = new System.Threading.Semaphore(0, 1);
            cache.addClientListener(cl, new string[] { }, new string[] { }, null);
            createdSemaphore.WaitOne();
            Assert.AreEqual(1, createdEventCounter);
            cache.removeClientListener(cl);
        }

    }
}