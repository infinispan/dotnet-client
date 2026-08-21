using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Infinispan.Hotrod
{
    public enum NearCacheMode
    {
        Disabled,
        Invalidated
    }

    public class NearCacheStats
    {
        public long Hits { get; internal set; }
        public long Misses { get; internal set; }
        public long Invalidations { get; internal set; }
        public int Size { get; internal set; }
    }

    internal class NearCache<K, V>
    {
        private readonly ConcurrentDictionary<K, CacheEntry> _entries = new();
        private readonly LinkedList<K> _lruList = new();
        private readonly object _lruLock = new();
        private readonly int _maxEntries;
        private long _hits;
        private long _misses;
        private long _invalidations;

        internal NearCache(int maxEntries)
        {
            _maxEntries = maxEntries;
        }

        internal bool TryGet(K key, out V value)
        {
            if (_entries.TryGetValue(key, out var entry))
            {
                Interlocked.Increment(ref _hits);
                Touch(entry);
                value = entry.Value;
                return true;
            }
            Interlocked.Increment(ref _misses);
            value = default;
            return false;
        }

        internal void Put(K key, V value)
        {
            if (_entries.TryGetValue(key, out var existing))
            {
                existing.Value = value;
                Touch(existing);
                return;
            }

            var entry = new CacheEntry { Value = value };
            if (_entries.TryAdd(key, entry))
            {
                lock (_lruLock)
                {
                    entry.Node = _lruList.AddFirst(key);
                    Evict();
                }
            }
            else if (_entries.TryGetValue(key, out existing))
            {
                existing.Value = value;
                Touch(existing);
            }
        }

        internal void Invalidate(K key)
        {
            if (_entries.TryRemove(key, out var entry))
            {
                Interlocked.Increment(ref _invalidations);
                lock (_lruLock)
                {
                    if (entry.Node?.List != null)
                        _lruList.Remove(entry.Node);
                }
            }
        }

        internal void Clear()
        {
            _entries.Clear();
            lock (_lruLock)
            {
                _lruList.Clear();
            }
        }

        internal int Count => _entries.Count;

        internal NearCacheStats GetStats()
        {
            return new NearCacheStats
            {
                Hits = Interlocked.Read(ref _hits),
                Misses = Interlocked.Read(ref _misses),
                Invalidations = Interlocked.Read(ref _invalidations),
                Size = _entries.Count
            };
        }

        private void Touch(CacheEntry entry)
        {
            lock (_lruLock)
            {
                if (entry.Node?.List != null)
                {
                    _lruList.Remove(entry.Node);
                    entry.Node = _lruList.AddFirst(entry.Node.Value);
                }
            }
        }

        private void Evict()
        {
            while (_entries.Count > _maxEntries && _lruList.Last != null)
            {
                var evictKey = _lruList.Last.Value;
                _lruList.RemoveLast();
                _entries.TryRemove(evictKey, out _);
            }
        }

        private class CacheEntry
        {
            public V Value;
            public LinkedListNode<K> Node;
        }
    }

    internal class NearCacheListener<K, V> : AbstractClientListener
    {
        private string _listenerId = Guid.NewGuid().ToString();
        private readonly NearCache<K, V> _nearCache;
        private readonly Marshaller<K> _keyMarshaller;

        internal NearCacheListener(NearCache<K, V> nearCache, Marshaller<K> keyMarshaller)
        {
            _nearCache = nearCache;
            _keyMarshaller = keyMarshaller;
        }

        public override string ListenerID { get => _listenerId; set => _listenerId = value; }

        public override void OnEvent(Event e)
        {
            if (e.Key != null)
                _nearCache.Invalidate(_keyMarshaller.unmarshall(e.Key));
            else
                _nearCache.Clear();
        }

        public override void OnError(Exception ex = null)
        {
            _nearCache.Clear();
        }
    }
}
