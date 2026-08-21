using System;
using System.Threading.Tasks;
using Infinispan.Hotrod.Tests.Util;
using Xunit;

namespace Infinispan.Hotrod.XUnitTest
{
    public class CounterTestFixture : IAsyncLifetime
    {
        private InfinispanContainer _container;
        public InfinispanClient infinispan = new InfinispanClient();

        public async Task InitializeAsync()
        {
            _container = new InfinispanContainer("infinispan-noauth.xml");
            await _container.StartAsync();
            infinispan.AddHost(_container.Host, _container.Port);
            infinispan.Version = ProtocolVersion.Version31;
            infinispan.ClientIntelligence = ClientIntelligence.Basic;
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }
    }

    [Collection("MainSequence")]
    public class CounterTest : IClassFixture<CounterTestFixture>
    {
        private readonly InfinispanClient _infinispan;
        private readonly CounterManager _counters;

        public CounterTest(CounterTestFixture fixture)
        {
            _infinispan = fixture.infinispan;
            _counters = _infinispan.Counters();
        }

        [Fact]
        public async Task DefineStrongCounterTest()
        {
            var name = $"strong-{Guid.NewGuid():N}";
            var created = await _counters.Define(name, new CounterConfiguration
            {
                Type = CounterType.Strong,
                Storage = CounterStorage.Volatile,
                InitialValue = 0
            });
            Assert.True(created);
        }

        [Fact]
        public async Task DefineWeakCounterTest()
        {
            var name = $"weak-{Guid.NewGuid():N}";
            var created = await _counters.Define(name, new CounterConfiguration
            {
                Type = CounterType.Weak,
                Storage = CounterStorage.Volatile,
                ConcurrencyLevel = 4,
                InitialValue = 0
            });
            Assert.True(created);
        }

        [Fact]
        public async Task DefineBoundedCounterTest()
        {
            var name = $"bounded-{Guid.NewGuid():N}";
            var created = await _counters.Define(name, new CounterConfiguration
            {
                Type = CounterType.Strong,
                Storage = CounterStorage.Volatile,
                Bounded = true,
                LowerBound = 0,
                UpperBound = 100,
                InitialValue = 50
            });
            Assert.True(created);

            var counter = _counters.Counter(name);
            Assert.Equal(50, await counter.Get());
        }

        [Fact]
        public async Task IsDefinedTest()
        {
            var name = $"defined-{Guid.NewGuid():N}";
            Assert.False(await _counters.IsDefined(name));

            await _counters.Define(name, new CounterConfiguration
            {
                Type = CounterType.Strong,
                Storage = CounterStorage.Volatile,
                InitialValue = 0
            });
            Assert.True(await _counters.IsDefined(name));
        }

        [Fact]
        public async Task GetConfigurationTest()
        {
            var name = $"config-{Guid.NewGuid():N}";
            await _counters.Define(name, new CounterConfiguration
            {
                Type = CounterType.Strong,
                Storage = CounterStorage.Persistent,
                Bounded = true,
                LowerBound = -10,
                UpperBound = 10,
                InitialValue = 5
            });

            var config = await _counters.GetConfiguration(name);
            Assert.NotNull(config);
            Assert.Equal(CounterType.Strong, config.Type);
            Assert.Equal(CounterStorage.Persistent, config.Storage);
            Assert.True(config.Bounded);
            Assert.Equal(-10, config.LowerBound);
            Assert.Equal(10, config.UpperBound);
            Assert.Equal(5, config.InitialValue);
        }

        [Fact]
        public async Task GetTest()
        {
            var name = $"get-{Guid.NewGuid():N}";
            await _counters.Define(name, new CounterConfiguration
            {
                Type = CounterType.Strong,
                Storage = CounterStorage.Volatile,
                InitialValue = 42
            });

            var counter = _counters.Counter(name);
            Assert.Equal(42, await counter.Get());
        }

        [Fact]
        public async Task AddAndGetTest()
        {
            var name = $"add-{Guid.NewGuid():N}";
            await _counters.Define(name, new CounterConfiguration
            {
                Type = CounterType.Strong,
                Storage = CounterStorage.Volatile,
                InitialValue = 10
            });

            var counter = _counters.Counter(name);
            Assert.Equal(15, await counter.AddAndGet(5));
            Assert.Equal(13, await counter.AddAndGet(-2));
        }

        [Fact]
        public async Task GetAndSetTest()
        {
            var name = $"getset-{Guid.NewGuid():N}";
            await _counters.Define(name, new CounterConfiguration
            {
                Type = CounterType.Strong,
                Storage = CounterStorage.Volatile,
                InitialValue = 10
            });

            var counter = _counters.Counter(name);
            var previous = await counter.GetAndSet(99);
            Assert.Equal(10, previous);
            Assert.Equal(99, await counter.Get());
        }

        [Fact]
        public async Task CompareAndSwapTest()
        {
            var name = $"cas-{Guid.NewGuid():N}";
            await _counters.Define(name, new CounterConfiguration
            {
                Type = CounterType.Strong,
                Storage = CounterStorage.Volatile,
                InitialValue = 10
            });

            var counter = _counters.Counter(name);

            var (oldValue, success) = await counter.CompareAndSwap(10, 20);
            Assert.True(success);
            Assert.Equal(10, oldValue);
            Assert.Equal(20, await counter.Get());

            var (oldValue2, success2) = await counter.CompareAndSwap(10, 30);
            Assert.False(success2);
            Assert.Equal(20, oldValue2);
            Assert.Equal(20, await counter.Get());
        }

        [Fact]
        public async Task ResetTest()
        {
            var name = $"reset-{Guid.NewGuid():N}";
            await _counters.Define(name, new CounterConfiguration
            {
                Type = CounterType.Strong,
                Storage = CounterStorage.Volatile,
                InitialValue = 10
            });

            var counter = _counters.Counter(name);
            await counter.AddAndGet(90);
            Assert.Equal(100, await counter.Get());

            await counter.Reset();
            Assert.Equal(10, await counter.Get());
        }

        [Fact]
        public async Task RemoveTest()
        {
            var name = $"remove-{Guid.NewGuid():N}";
            await _counters.Define(name, new CounterConfiguration
            {
                Type = CounterType.Strong,
                Storage = CounterStorage.Volatile,
                InitialValue = 5
            });

            var counter = _counters.Counter(name);
            await counter.AddAndGet(10);
            Assert.Equal(15, await counter.Get());

            await _counters.Remove(name);

            // After remove, value resets to the initial value
            Assert.Equal(5, await counter.Get());
        }

        [Fact]
        public async Task NamesTest()
        {
            var name = $"names-{Guid.NewGuid():N}";
            await _counters.Define(name, new CounterConfiguration
            {
                Type = CounterType.Strong,
                Storage = CounterStorage.Volatile,
                InitialValue = 0
            });

            var names = await _counters.Names();
            Assert.Contains(name, names);
        }
    }
}
