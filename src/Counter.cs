using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infinispan.Hotrod
{
    public enum CounterType : byte
    {
        Strong = 0,
        Weak = 1
    }

    public enum CounterStorage : byte
    {
        Volatile = 0,
        Persistent = 1
    }

    public enum CounterState : byte
    {
        Valid = 0,
        LowerBound = 1,
        UpperBound = 2
    }

    public class CounterConfiguration
    {
        public CounterType Type { get; set; }
        public bool Bounded { get; set; }
        public CounterStorage Storage { get; set; }
        public int ConcurrencyLevel { get; set; }
        public long LowerBound { get; set; }
        public long UpperBound { get; set; }
        public long InitialValue { get; set; }

        internal byte EncodeFlags()
        {
            byte flags = 0;
            if (Type == CounterType.Weak) flags |= 0x01;
            if (Bounded) flags |= 0x02;
            if (Storage == CounterStorage.Persistent) flags |= 0x04;
            return flags;
        }

        internal static CounterConfiguration Decode(byte flags, ResponseStream stream)
        {
            var config = new CounterConfiguration();
            if ((flags & 0x01) != 0) config.Type = CounterType.Weak;
            config.Bounded = (flags & 0x02) != 0;
            if ((flags & 0x04) != 0) config.Storage = CounterStorage.Persistent;
            if (config.Type == CounterType.Weak)
                config.ConcurrencyLevel = Codec.readVInt(stream);
            if (config.Bounded)
            {
                config.LowerBound = Codec.readLong(stream);
                config.UpperBound = Codec.readLong(stream);
            }
            config.InitialValue = Codec.readLong(stream);
            return config;
        }
    }

    public class CounterManager
    {
        private readonly InfinispanClient _client;

        internal CounterManager(InfinispanClient client)
        {
            _client = client;
        }

        public async Task<bool> Define(string name, CounterConfiguration config)
        {
            return await _client.CounterCreate(name, config);
        }

        public async Task<bool> IsDefined(string name)
        {
            return await _client.CounterIsDefined(name);
        }

        public async Task<CounterConfiguration> GetConfiguration(string name)
        {
            return await _client.CounterGetConfiguration(name);
        }

        public async Task Remove(string name)
        {
            await _client.CounterRemove(name);
        }

        public async Task<IList<string>> Names()
        {
            return await _client.CounterGetNames();
        }

        public Counter Counter(string name)
        {
            return new Counter(_client, name);
        }
    }

    public class Counter
    {
        private readonly InfinispanClient _client;

        public string Name { get; }

        internal Counter(InfinispanClient client, string name)
        {
            _client = client;
            Name = name;
        }

        public async Task<long> Get()
        {
            return await _client.CounterGet(Name);
        }

        public async Task<long> AddAndGet(long delta)
        {
            return await _client.CounterAddAndGet(Name, delta);
        }

        public async Task<long> GetAndSet(long value)
        {
            return await _client.CounterGetAndSet(Name, value);
        }

        public async Task<(long OldValue, bool Success)> CompareAndSwap(long expect, long update)
        {
            return await _client.CounterCompareAndSwap(Name, expect, update);
        }

        public async Task Reset()
        {
            await _client.CounterReset(Name);
        }
    }
}
