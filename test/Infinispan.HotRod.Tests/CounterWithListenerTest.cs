using System;
using Infinispan.HotRod.Exceptions;
using Infinispan.HotRod.Config;
using NUnit.Framework;
using System.Threading;

namespace Infinispan.HotRod.Tests.ClusteredIndexingXml
{
    [TestFixture]
    [Category("clustered_indexing_xml")]
    [Category("CounterWithListenerTestSuite")]
    class CounterWithListenerTest
    {
        RemoteCacheManager remoteManager;
        [OneTimeSetUp]
        public void BeforeClass()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222);
            conf.ConnectionTimeout(90000).SocketTimeout(6000);
            conf.ProtocolVersion("2.7");
            remoteManager = new RemoteCacheManager(conf.Build(), true);
        }

        [OneTimeTearDown]
        public void AfterClass()
        {
            remoteManager.Stop();
        }

        [Test]
        [Ignore("Listeners on counters need review")]
        public void AddRemoveListenerWeakTest()
        {
            Semaphore s = new Semaphore(0, 1);
            bool secondEvent = false;
            Action<Event.CounterEvent> a = (Event.CounterEvent e) =>
            {
                if (!secondEvent)
                {
                    Assert.AreEqual(5, e.OldValue);
                    Assert.AreEqual(15, e.NewValue);
                    Assert.AreEqual(Event.CounterState.VALID, e.NewState);
                    Assert.AreEqual(Event.CounterState.VALID, e.OldState);
                    secondEvent = true;
                }
                s.Release();
            };

            Action<Event.CounterEvent> a2 = (Event.CounterEvent e) =>
            {
                Assert.AreEqual(6, e.OldValue);
                Assert.AreEqual(16, e.NewValue);
                Assert.AreEqual(Event.CounterState.VALID, e.NewState);
                Assert.AreEqual(Event.CounterState.VALID, e.OldState);
                s.Release();
            };

            string counterName = "weakTestAddRemoveListener";
            Event.CounterListener cl = new Event.CounterListener(counterName, a);
            Event.CounterListener cl1 = new Event.CounterListener(counterName, a2);
            var rcm = remoteManager.GetCounterManager();
            var cc = new CounterConfiguration(5, 0, 20, 8, CounterType.WEAK, Storage.VOLATILE);
            rcm.DefineCounter(counterName, cc);
            var counter = rcm.GetWeakCounter(counterName);
            object o = counter.AddListener(cl);
            counter.Add(10);
            Assert.True(s.WaitOne(5000));
            counter.RemoveListener(o);
            counter.Add(-9);
            Assert.False(s.WaitOne(5000));

            object o1 = counter.AddListener(cl1);
            counter.Add(10);
            Assert.True(s.WaitOne(5000));
            counter.RemoveListener(o1);

        }

        [Test]
        [Ignore("ISPN-9296")]
        public void AddRemoveCounterWithListenerWeakTest()
        {
            Semaphore s = new Semaphore(0, 1);
            Action<Event.CounterEvent> a = (Event.CounterEvent e) =>
            {
                s.Release();
            };

            string counterName = "weak1TestAddRemoveListener";
            Event.CounterListener cl = new Event.CounterListener(counterName, a);
            var rcm = remoteManager.GetCounterManager();
            var cc = new CounterConfiguration(5, 0, 20, 8, CounterType.WEAK, Storage.VOLATILE);
            rcm.DefineCounter(counterName, cc);
            var counter = rcm.GetWeakCounter(counterName);
            object o = counter.AddListener(cl);
            counter.Add(10);
            Assert.True(s.WaitOne(5000));
            counter.Remove();

            // After the remove no events should arrive
            changeCounterFromAnotherCacheManager();
            Assert.False(s.WaitOne(5000));

            // if the counter is recreated, events should come again
            var counter1 = rcm.GetWeakCounter(counterName);
            counter1.Add(10);
            Assert.AreEqual(15, counter1.GetValue());
            Assert.True(s.WaitOne(5000));
            //counter.RemoveListener(o);
        }

        void changeCounterFromAnotherCacheManager()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222);
            conf.ConnectionTimeout(90000).SocketTimeout(6000);
            conf.ProtocolVersion("2.7");
            RemoteCacheManager remoteManager = new RemoteCacheManager(conf.Build(), true);

            var rcm = remoteManager.GetCounterManager();
            string counterName = "weak1TestAddRemoveListener";
            var counter = rcm.GetWeakCounter(counterName);
            counter.Increment();
            remoteManager.Stop();
        }

        [Test]
        [Ignore("Listeners on counters need review")]
        public void BasicListenerStrongTest()
        {
            int step = 0;
            Semaphore s = new Semaphore(0, 1);
            Action<Event.CounterEvent> a2 = (Event.CounterEvent e) =>
            {
                switch (step)
                {
                    case 0:
                        Assert.AreEqual(5, e.OldValue);
                        Assert.AreEqual(15, e.NewValue);
                        Assert.AreEqual(Event.CounterState.VALID, e.NewState);
                        Assert.AreEqual(Event.CounterState.VALID, e.OldState);
                        s.Release();
                        break;
                    case 1:
                        Assert.AreEqual(15, e.OldValue);
                        Assert.AreEqual(0, e.NewValue);
                        Assert.AreEqual(Event.CounterState.LOWER_BOUND_REACHED, e.NewState);
                        Assert.AreEqual(Event.CounterState.VALID, e.OldState);
                        s.Release();
                        break;
                    case 2:
                        Assert.AreEqual(0, e.OldValue);
                        Assert.AreEqual(20, e.NewValue);
                        Assert.AreEqual(Event.CounterState.UPPER_BOUND_REACHED, e.NewState);
                        Assert.AreEqual(Event.CounterState.LOWER_BOUND_REACHED, e.OldState);
                        s.Release();
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            };


            string counterName = "strongTestBasicListener";
            Event.CounterListener cl = new Event.CounterListener(counterName, a2);
            var rcm = remoteManager.GetCounterManager();
            var cc = new CounterConfiguration(5, 0, 20, 8, CounterType.BOUNDED_STRONG, Storage.VOLATILE);
            rcm.DefineCounter(counterName, cc);
            var counter = rcm.GetStrongCounter(counterName);
            object o = counter.AddListener(cl);
            counter.AddAndGet(10);
            Assert.True(s.WaitOne(15000));
            step = 1;
            try
            {
                counter.AddAndGet(-20);
            }
            catch (CounterLowerBoundException)
            {
            }
            Assert.True(s.WaitOne(15000));
            step = 2;
            try
            {
                counter.AddAndGet(30);
            }
            catch (CounterUpperBoundException)
            {
            }
            Assert.True(s.WaitOne(15000));
            counter.RemoveListener(o);
        }
    }
}
