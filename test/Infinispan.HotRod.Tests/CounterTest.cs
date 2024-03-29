﻿using System;
using Infinispan.HotRod.Exceptions;
using Infinispan.HotRod.Config;
using NUnit.Framework;
using System.Threading;

namespace Infinispan.HotRod.Tests.ClusteredIndexingXml
{
    [TestFixture]
    [Category("clustered_indexing_xml")]
    [Category("CounterTestSuite")]
    class CounterTest
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
        public void ResetStrongTest()
        {
            string counterName = "strongTestReset";
            int initialValue = 5;
            var rcm = remoteManager.GetCounterManager();
            var cc = new CounterConfiguration(initialValue, 0, 200, 8, CounterType.BOUNDED_STRONG, Storage.VOLATILE);
            rcm.DefineCounter(counterName, cc);
            var counter = rcm.GetStrongCounter(counterName);
            int val = counter.GetValue();
            var name = counter.GetName();
            Assert.AreEqual(initialValue, counter.GetValue());
            rcm.Remove(counterName);
        }

        [Test]
        public void ConfigurationStrongTest()
        {
            string counterName = "strongTestConfiguration";
            var rcm = remoteManager.GetCounterManager();
            var cc = new CounterConfiguration(0, -256, 65536, 64, CounterType.BOUNDED_STRONG, Storage.VOLATILE);
            rcm.DefineCounter(counterName, cc);
            var counter = rcm.GetStrongCounter(counterName);
            ICounterConfiguration retCc = counter.GetConfiguration();
            Assert.AreEqual(cc.GetLowerBound(), retCc.GetLowerBound());
            Assert.AreEqual(cc.GetUpperBound(), retCc.GetUpperBound());
            Assert.AreEqual(0, retCc.GetConcurrencyLevel());
            Assert.AreEqual(cc.GetType(), retCc.GetType());
            Assert.AreEqual(cc.GetStorage(), retCc.GetStorage());
            Assert.AreEqual(counter.GetName(), counterName);
            rcm.Remove(counterName);

            string counterName1 = "strongTestConfiguration1";
            var cc1 = new CounterConfiguration(0, -100, 100, 1, CounterType.UNBOUNDED_STRONG,
                        Storage.PERSISTENT);
            rcm.DefineCounter(counterName1, cc1);
            counter = rcm.GetStrongCounter(counterName1);
            ICounterConfiguration retCc1 = counter.GetConfiguration();
            Assert.AreEqual(0, retCc1.GetLowerBound());
            Assert.AreEqual(0, retCc1.GetUpperBound());
            Assert.AreEqual(0, retCc1.GetConcurrencyLevel());
            Assert.AreEqual(cc1.GetType(), retCc1.GetType());
            Assert.AreEqual(cc1.GetStorage(), retCc1.GetStorage());
            Assert.AreEqual(counter.GetName(), counterName1);
            rcm.Remove(counterName1);
        }

        [Test]
        public void RemoveStrongTest()
        {
            string counterName = "strongTestRemove";
            int initialValue = 5;
            var rcm = remoteManager.GetCounterManager();
            var cc = new CounterConfiguration(initialValue, -100, 200, 8, CounterType.UNBOUNDED_STRONG, Storage.VOLATILE);
            rcm.DefineCounter(counterName, cc);
            var counter = rcm.GetStrongCounter(counterName);
            Assert.AreEqual(counter.AddAndGet(100), 105);
            rcm.Remove(counterName);
            Assert.AreEqual(counter.GetValue(), 5);
            Assert.AreEqual(counter.AddAndGet(-100), -95);
            rcm.Remove(counterName);
            Assert.AreEqual(counter.GetValue(), 5);
            rcm.Remove(counterName);
        }

        [Test]
        public void AddStrongTest()
        {
            string counterName = "strongTestAdd";
            int initialValue = 10;
            var rcm = remoteManager.GetCounterManager();
            var cc = new CounterConfiguration(initialValue, 0, 0, 8, CounterType.UNBOUNDED_STRONG, Storage.VOLATILE);
            rcm.DefineCounter(counterName, cc);
            var counter = rcm.GetStrongCounter(counterName);
            Assert.AreEqual(counter.GetValue(), 10);
            Assert.AreEqual(counter.AddAndGet(10), 20);
            Assert.AreEqual(counter.AddAndGet(-20), 0);
            Assert.AreEqual(counter.AddAndGet(-20), -20);
            rcm.Remove(counterName);
        }

        [Test]
        public void CompareAndSetStrongTest()
        {
            string counterName = "strongTestCompareAndSet";
            int initialValue = 2;
            var rcm = remoteManager.GetCounterManager();
            var cc = new CounterConfiguration(initialValue, 0, 0, 0, CounterType.UNBOUNDED_STRONG, Storage.VOLATILE);
            rcm.DefineCounter(counterName, cc);
            var counter = rcm.GetStrongCounter(counterName);
            Assert.False(counter.CompareAndSet(0, 1));
            Assert.True(counter.CompareAndSet(2, 3));
            Assert.True(counter.CompareAndSet(3, 4));
            rcm.Remove(counterName);
        }

        [Test]
        public void CompareAndSwapStrongTest()
        {
            string counterName = "strongTestCompareAndSwap";
            int initialValue = 3;
            var rcm = remoteManager.GetCounterManager();
            var cc = new CounterConfiguration(initialValue, 0, 0, 0, CounterType.UNBOUNDED_STRONG, Storage.VOLATILE);
            rcm.DefineCounter(counterName, cc);
            var counter = rcm.GetStrongCounter(counterName);
            Assert.AreEqual(3, counter.CompareAndSwap(1, 2));
            Assert.AreEqual(3, counter.CompareAndSwap(3, 2));
            Assert.AreEqual(2, counter.CompareAndSwap(3, 4));
            Assert.AreEqual(2, counter.CompareAndSwap(2, 5));
            rcm.Remove(counterName);
        }

        [Test]
        public void BoundariesStrongTest()
        {
            string counterName = "strongTestBoundaries";
            int initialValue = 1;
            var rcm = remoteManager.GetCounterManager();
            var cc = new CounterConfiguration(initialValue, 0, 20, 0, CounterType.BOUNDED_STRONG, Storage.VOLATILE);
            rcm.DefineCounter(counterName, cc);
            var counter = rcm.GetStrongCounter(counterName);
            Assert.AreEqual(1, counter.GetValue());
            Assert.Throws<Infinispan.HotRod.Exceptions.CounterLowerBoundException>(() =>
            { counter.AddAndGet(-10); });
            Assert.Throws<Infinispan.HotRod.Exceptions.CounterUpperBoundException>(() =>
            { counter.AddAndGet(30); });
            Assert.Throws<Infinispan.HotRod.Exceptions.CounterLowerBoundException>(() =>
            { counter.CompareAndSet(20, -1); });
            Assert.Throws<Infinispan.HotRod.Exceptions.CounterUpperBoundException>(() =>
            { counter.CompareAndSet(20, 21); });
            Assert.Throws<Infinispan.HotRod.Exceptions.CounterLowerBoundException>(() =>
            { counter.CompareAndSwap(20, -1); });
            Assert.Throws<Infinispan.HotRod.Exceptions.CounterUpperBoundException>(() =>
            { counter.CompareAndSwap(20, 21); });
            rcm.Remove(counterName);
        }

        [Test]
        public void AddWeakTest()
        {
            string counterName = "weakTestAdd";
            int initialValue = 10;
            var rcm = remoteManager.GetCounterManager();
            var cc = new CounterConfiguration(initialValue, 0, 0, 8, CounterType.WEAK, Storage.VOLATILE);
            rcm.DefineCounter(counterName, cc);
            var counter = rcm.GetWeakCounter(counterName);
            Assert.AreEqual(10, counter.GetValue());
            counter.Add(10);
            Assert.AreEqual(20, counter.GetValue());
            counter.Add(-20);
            Assert.AreEqual(0, counter.GetValue());
            counter.Add(-20);
            Assert.AreEqual(-20, counter.GetValue());
            rcm.Remove(counterName);
        }

        [Test]
        public void ResetWeakTest()
        {
            string counterName = "weakTestReset";
            int initialValue = 5;
            var rcm = remoteManager.GetCounterManager();
            var cc = new CounterConfiguration(initialValue, 0, 0, 8, CounterType.WEAK, Storage.VOLATILE);
            rcm.DefineCounter(counterName, cc);
            var counter = rcm.GetWeakCounter(counterName);
            Assert.AreEqual(5, counter.GetValue());
            counter.Add(100);
            Assert.AreEqual(105, counter.GetValue());
            counter.Reset();
            Assert.AreEqual(counter.GetValue(), initialValue);
            rcm.Remove(counterName);
        }

        [Test]
        public void ConfigurationWeakTest()
        {
            string counterName = "weakTestConfiguration";
            var rcm = remoteManager.GetCounterManager();
            var cc = new CounterConfiguration(0, 0, 0, 64, CounterType.WEAK, Storage.VOLATILE);
            rcm.DefineCounter(counterName, cc);
            var counter = rcm.GetWeakCounter(counterName);
            ICounterConfiguration retCc = counter.GetConfiguration();
            Assert.AreEqual(cc.GetConcurrencyLevel(), retCc.GetConcurrencyLevel());
            Assert.AreEqual(cc.GetType(), retCc.GetType());
            Assert.AreEqual(cc.GetStorage(), retCc.GetStorage());
            Assert.AreEqual(counter.GetName(), counterName);
            rcm.Remove(counterName);

            string counterName1 = "weakTestConfiguration1";
            var cc1 = new CounterConfiguration(0, 0, 0, 1, CounterType.WEAK, Storage.PERSISTENT);
            rcm.DefineCounter(counterName1, cc1);
            counter = rcm.GetWeakCounter(counterName1);
            ICounterConfiguration retCc1 = counter.GetConfiguration();
            Assert.AreEqual(cc1.GetConcurrencyLevel(), retCc1.GetConcurrencyLevel());
            Assert.AreEqual(cc1.GetType(), retCc1.GetType());
            Assert.AreEqual(cc1.GetStorage(), retCc1.GetStorage());
            Assert.AreEqual(counter.GetName(), counterName1);
            rcm.Remove(counterName1);
        }

        [Test]
        public void RemoveWeakTest()
        {
            string counterName = "weakTestRemove";
            var rcm = remoteManager.GetCounterManager();
            var cc = new CounterConfiguration(5, 0, 0, 8, CounterType.WEAK, Storage.VOLATILE);
            rcm.DefineCounter(counterName, cc);
            var counter = rcm.GetWeakCounter(counterName);
            counter.Add(100);
            Assert.AreEqual(105, counter.GetValue());
            rcm.Remove(counterName);
            Assert.AreEqual(5, counter.GetValue());
            counter.Add(-100);
            Assert.AreEqual(-95, counter.GetValue());
            rcm.Remove(counterName);
            Assert.AreEqual(5, counter.GetValue());
            rcm.Remove(counterName);
        }
    }
}
