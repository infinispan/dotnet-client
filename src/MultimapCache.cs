using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infinispan.Hotrod
{
    public class MultimapCache<K, V>
    {
        private readonly InfinispanClient _client;
        private readonly CacheBase _cache;
        private readonly Marshaller<K> _keyMarshaller;
        private readonly Marshaller<V> _valueMarshaller;
        private readonly bool _supportsDuplicates;

        internal MultimapCache(InfinispanClient client, Marshaller<K> keyMarshaller,
            Marshaller<V> valueMarshaller, string name, bool supportsDuplicates)
        {
            _client = client;
            _keyMarshaller = keyMarshaller;
            _valueMarshaller = valueMarshaller;
            _supportsDuplicates = supportsDuplicates;
            _cache = new CacheBase(client, name);
        }

        public string Name => _cache.Name;

        public MediaType KeyMediaType
        {
            get => _cache.KeyMediaType;
            set => _cache.KeyMediaType = value;
        }

        public MediaType ValueMediaType
        {
            get => _cache.ValueMediaType;
            set => _cache.ValueMediaType = value;
        }

        public async Task Put(K key, V value)
        {
            await _client.MultimapPut(_cache,
                _keyMarshaller.marshall(key),
                _valueMarshaller.marshall(value),
                _supportsDuplicates);
        }

        public async Task<IList<V>> Get(K key)
        {
            var rawValues = await _client.MultimapGet(_cache,
                _keyMarshaller.marshall(key), _supportsDuplicates);
            var result = new List<V>(rawValues.Count);
            foreach (var raw in rawValues)
                result.Add(_valueMarshaller.unmarshall(raw));
            return result;
        }

        public async Task<bool> RemoveKey(K key)
        {
            return await _client.MultimapRemoveKey(_cache,
                _keyMarshaller.marshall(key), _supportsDuplicates);
        }

        public async Task<bool> RemoveEntry(K key, V value)
        {
            return await _client.MultimapRemoveEntry(_cache,
                _keyMarshaller.marshall(key),
                _valueMarshaller.marshall(value),
                _supportsDuplicates);
        }

        public async Task<bool> ContainsKey(K key)
        {
            return await _client.MultimapContainsKey(_cache,
                _keyMarshaller.marshall(key), _supportsDuplicates);
        }

        public async Task<bool> ContainsValue(V value)
        {
            return await _client.MultimapContainsValue(_cache,
                _valueMarshaller.marshall(value), _supportsDuplicates);
        }

        public async Task<bool> ContainsEntry(K key, V value)
        {
            return await _client.MultimapContainsEntry(_cache,
                _keyMarshaller.marshall(key),
                _valueMarshaller.marshall(value),
                _supportsDuplicates);
        }

        public async Task<long> Size()
        {
            return await _client.MultimapSize(_cache, _supportsDuplicates);
        }
    }
}
