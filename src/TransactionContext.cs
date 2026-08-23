using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infinispan.Hotrod
{
    public class TransactionContext<K, V>
    {
        private readonly Cache<K, V> _cache;
        private readonly Marshaller<K> _keyMarshaller;
        private readonly Marshaller<V> _valueMarshaller;
        private readonly long _timeoutMs;
        private readonly Xid _xid;
        private readonly Dictionary<string, TransactionEntry> _entries = new();
        private bool _completed;

        internal TransactionContext(Cache<K, V> cache, long timeoutMs)
        {
            _cache = cache;
            _timeoutMs = timeoutMs;
            _xid = Xid.Create();
            _keyMarshaller = cache.KeyMarshaller;
            _valueMarshaller = cache.ValueMarshaller;
        }

        private string KeyToString(K key) => Convert.ToBase64String(_keyMarshaller.Marshall(key));

        /// <summary>
        /// Get a value within the transaction. Reads from local buffer first,
        /// falls back to the cache and tracks the version for optimistic locking.
        /// </summary>
        public async Task<V> Get(K key)
        {
            ThrowIfCompleted();
            var keyStr = KeyToString(key);
            if (_entries.TryGetValue(keyStr, out var entry))
            {
                if (entry.Removed)
                    return default;
                return entry.Value != null ? _valueMarshaller.Unmarshall(entry.Value) : default;
            }

            var vwv = await _cache.GetWithVersion(key);
            var txEntry = new TransactionEntry { Read = true };
            if (vwv != null)
            {
                txEntry.Existed = true;
                txEntry.Version = vwv.Version;
                txEntry.Value = _valueMarshaller.Marshall(vwv.Value);
            }
            _entries[keyStr] = txEntry;
            return vwv != null ? vwv.Value : default;
        }

        /// <summary>
        /// Buffer a put operation within the transaction
        /// </summary>
        public async Task Put(K key, V value, ExpirationTime lifespan = null, ExpirationTime maxidle = null)
        {
            ThrowIfCompleted();
            var keyStr = KeyToString(key);
            if (!_entries.TryGetValue(keyStr, out var entry))
            {
                var vwv = await _cache.GetWithVersion(key);
                entry = new TransactionEntry { Read = true };
                if (vwv != null)
                {
                    entry.Existed = true;
                    entry.Version = vwv.Version;
                }
                _entries[keyStr] = entry;
            }
            entry.Value = _valueMarshaller.Marshall(value);
            entry.Removed = false;
            entry.Lifespan = lifespan;
            entry.MaxIdle = maxidle;
        }

        /// <summary>
        /// Buffer a put operation without reading the current value (blind write)
        /// </summary>
        public void PutBlind(K key, V value, ExpirationTime lifespan = null, ExpirationTime maxidle = null)
        {
            ThrowIfCompleted();
            var keyStr = KeyToString(key);
            if (!_entries.TryGetValue(keyStr, out var entry))
            {
                entry = new TransactionEntry { Read = false };
                _entries[keyStr] = entry;
            }
            entry.Value = _valueMarshaller.Marshall(value);
            entry.Removed = false;
            entry.Lifespan = lifespan;
            entry.MaxIdle = maxidle;
        }

        /// <summary>
        /// Buffer a remove operation within the transaction
        /// </summary>
        public async Task Remove(K key)
        {
            ThrowIfCompleted();
            var keyStr = KeyToString(key);
            if (!_entries.TryGetValue(keyStr, out var entry))
            {
                var vwv = await _cache.GetWithVersion(key);
                entry = new TransactionEntry { Read = true };
                if (vwv != null)
                {
                    entry.Existed = true;
                    entry.Version = vwv.Version;
                }
                _entries[keyStr] = entry;
            }
            entry.Removed = true;
        }

        /// <summary>
        /// Commit the transaction: prepare + commit (two-phase) or one-phase if no other modifications
        /// </summary>
        /// <returns>XA return code (0 = XA_OK)</returns>
        public async Task<int> CommitAsync()
        {
            ThrowIfCompleted();
            _completed = true;

            var modifications = BuildModifications();
            if (modifications.Count == 0)
                return XaReturnCode.XA_RDONLY;

            bool onePhaseCommit = true;
            var (xaCode, shouldRetry) = await _cache.Cluster.PrepareTx(_cache, _xid, onePhaseCommit, modifications, false, _timeoutMs);

            if (shouldRetry)
                throw new InfinispanException("Transaction prepare failed: conflict detected");

            if (xaCode != XaReturnCode.XA_OK && xaCode != XaReturnCode.XA_RDONLY)
                throw new InfinispanException($"Transaction prepare failed with XA code: {xaCode}");

            return xaCode;
        }

        /// <summary>
        /// Commit the transaction using two-phase commit protocol
        /// </summary>
        /// <returns>XA return code (0 = XA_OK)</returns>
        public async Task<int> CommitTwoPhaseAsync()
        {
            ThrowIfCompleted();
            _completed = true;

            var modifications = BuildModifications();
            if (modifications.Count == 0)
                return XaReturnCode.XA_RDONLY;

            var (xaCode, shouldRetry) = await _cache.Cluster.PrepareTx(_cache, _xid, false, modifications, false, _timeoutMs);

            if (shouldRetry)
                throw new InfinispanException("Transaction prepare failed: conflict detected");

            if (xaCode != XaReturnCode.XA_OK && xaCode != XaReturnCode.XA_RDONLY)
                throw new InfinispanException($"Transaction prepare failed with XA code: {xaCode}");

            if (xaCode == XaReturnCode.XA_RDONLY)
                return xaCode;

            return await _cache.Cluster.CommitTx(_cache, _xid);
        }

        /// <summary>
        /// Rollback the transaction
        /// </summary>
        /// <returns>XA return code</returns>
        public async Task<int> RollbackAsync()
        {
            ThrowIfCompleted();
            _completed = true;

            var modifications = BuildModifications();
            if (modifications.Count == 0)
                return XaReturnCode.XA_OK;

            var (xaCode, _) = await _cache.Cluster.PrepareTx(_cache, _xid, false, modifications, false, _timeoutMs);

            return await _cache.Cluster.RollbackTx(_cache, _xid);
        }

        private List<TransactionModification> BuildModifications()
        {
            var result = new List<TransactionModification>();
            foreach (var kvp in _entries)
            {
                var keyBytes = Convert.FromBase64String(kvp.Key);
                var entry = kvp.Value;

                bool hasWrite = entry.Value != null || entry.Removed;
                if (!hasWrite)
                    continue;

                byte control = 0;
                if (!entry.Read)
                    control |= (byte)ControlByte.NOT_READ;
                else if (!entry.Existed)
                    control |= (byte)ControlByte.NON_EXISTING;
                if (entry.Removed)
                    control |= (byte)ControlByte.REMOVE_OP;

                var mod = new TransactionModification
                {
                    Key = keyBytes,
                    Control = control,
                    VersionRead = entry.Version,
                    Value = entry.Value,
                    Lifespan = entry.Lifespan ?? new ExpirationTime { Unit = TimeUnit.DEFAULT, Value = 0 },
                    MaxIdle = entry.MaxIdle ?? new ExpirationTime { Unit = TimeUnit.DEFAULT, Value = 0 }
                };
                result.Add(mod);
            }
            return result;
        }

        private void ThrowIfCompleted()
        {
            if (_completed)
                throw new InvalidOperationException("Transaction has already been committed or rolled back");
        }
    }
}
