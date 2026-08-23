using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Org.Infinispan.Protostream;
using Org.Infinispan.Query.Remote.Client;

namespace Infinispan.Hotrod
{
    public class CacheBase
    {
        public CacheBase(InfinispanClient ispnCluster, string name)
        {
            _cluster = ispnCluster;
            Name = name;
            NameAsBytes = Encoding.ASCII.GetBytes(Name);
            if (_cluster != null)
            {
                ForceReturnValue = _cluster.ForceReturnValue;
            }
            codec = Codec.getCodec((byte)_cluster.Version);
        }
        public readonly string Name;
        public bool ForceReturnValue;
        public MediaType KeyMediaType;
        public MediaType ValueMediaType;
        public readonly byte[] NameAsBytes;
        private readonly InfinispanClient _cluster;
        public InfinispanClient Cluster { get { return _cluster; } }
        public bool UseCacheDefaultLifespan;
        public bool UseCacheDefaultMaxIdle;
        public readonly Codec30 codec;
        public Int32 Flags { get { return getFlags(); } }
        private int getFlags()
        {
            int retVal = 0;
            if (ForceReturnValue)
                retVal += 1;
            if (UseCacheDefaultLifespan)
                retVal += 2;
            if (UseCacheDefaultMaxIdle)
                retVal += 4;
            return retVal;
        }
    }
    public class Cache<K, V> : CacheBase
    {
        public Cache(InfinispanClient ispnCluster, Marshaller<K> keyM, Marshaller<V> valM, string name) : base(ispnCluster, name)
        {
            KeyMarshaller = keyM;
            ValueMarshaller = valM;
        }
        internal readonly Marshaller<K> KeyMarshaller;
        internal readonly Marshaller<V> ValueMarshaller;
        private NearCache<K, V> _nearCache;
        private NearCacheListener<K, V> _nearCacheListener;

        /// <summary>
        /// Get an entry from the cache
        /// </summary>
        /// <param name="key">key of the entry</param>
        /// <returns>the value of the entry or null (async)</returns>
        public async Task<V> Get(K key)
        {
            if (_nearCache != null && _nearCache.TryGet(key, out var cached))
                return cached;
            var value = await Cluster.Get(KeyMarshaller, ValueMarshaller, (CacheBase)this, key);
            _nearCache?.Put(key, value);
            return value;
        }
        /// <summary>
        /// Get an entry from the cache with its version
        /// </summary>
        /// <param name="key">key of the entry</param>
        /// <returns>the value with version of the entry or null (async)</returns>
        public async Task<ValueWithVersion<V>> GetWithVersion(K key)
        {
            return await Cluster.GetWithVersion(KeyMarshaller, ValueMarshaller, (CacheBase)this, key);
        }
        /// <summary>
        /// Get an entry from the cache with its metadata
        /// </summary>
        /// <param name="key">key of the entry</param>
        /// <returns>the value with metadata of the entry or null (async)</returns>
        public async Task<ValueWithMetadata<V>> GetWithMetadata(K key)
        {
            return await Cluster.GetWithMetadata(KeyMarshaller, ValueMarshaller, (CacheBase)this, key);
        }
        /// <summary>
        /// Put/replace an entry in the cache
        /// </summary>
        /// <param name="key">key of the entry</param>
        /// <param name="value">value of the entry</param>
        /// <param name="lifespan">lifespan</param>
        /// <param name="maxidle">maximum idle time</param>
        /// <returns></returns>
        public async Task<V> Put(K key, V value, ExpirationTime lifespan = null, ExpirationTime maxidle = null)
        {
            var prev = await Cluster.Put(KeyMarshaller, ValueMarshaller, this, key, value, lifespan, maxidle);
            _nearCache?.Invalidate(key);
            return prev;
        }
        /// <summary>
        /// Put an entry in the cache if absent, does nothing otherwise
        /// </summary>
        /// <param name="key">key of the entry</param>
        /// <param name="value">value of the entry</param>
        /// <param name="lifespan">lifespan</param>
        /// <param name="maxidle">maximum idle time</param>
        /// <returns></returns>
        public async Task<V> PutIfAbsent(K key, V value, ExpirationTime lifespan = null, ExpirationTime maxidle = null)
        {
            var prev = await Cluster.PutIfAbsent(KeyMarshaller, ValueMarshaller, this, key, value, lifespan, maxidle);
            _nearCache?.Invalidate(key);
            return prev;
        }
        /// <summary>
        /// Return the number of entries in a cache
        /// </summary>
        /// <returns>number of entries</returns>
        public async Task<Int32> Size()
        {
            return await Cluster.Size(this);
        }
        /// <summary>
        /// Check if an entry with the given key is present
        /// </summary>
        /// <param name="key">key of the entry</param>
        /// <returns>true if an entry with the given exists</returns>
        public async Task<Boolean> ContainsKey(K key)
        {
            return await Cluster.ContainsKey(KeyMarshaller, (CacheBase)this, key);
        }
        /// <summary>
        /// Remove an entry from the cache
        /// </summary>
        /// <param name="key">entry's key</param>
        /// <returns>true if the entry has been removed</returns>
        public async Task<(V PrevValue, Boolean Removed)> Remove(K key)
        {
            var result = await Cluster.Remove(KeyMarshaller, ValueMarshaller, (CacheBase)this, key);
            _nearCache?.Invalidate(key);
            return result;
        }
        /// <summary>
        /// Clear the cache
        /// </summary>
        public async Task Clear()
        {
            await Cluster.Clear(this);
            _nearCache?.Clear();
        }
        /// <summary>
        /// Return true is the cache is empty
        /// </summary>
        public async Task<Boolean> IsEmpty()
        {
            return await Cluster.Size(this) == 0;
        }
        /// <summary>
        /// Acquire some cache/cluster statistics
        /// </summary>
        /// <returns>some statistics</returns>
        public async Task<ServerStatistics> Stats()
        {
            return await Cluster.Stats(this);
        }
        /// <summary>
        /// Replace an entry value
        /// </summary>
        /// <param name="key">entry key</param>
        /// <param name="value">new value</param>
        /// <param name="lifespan">lifespan for the entry</param>
        /// <param name="maxidle">max idle time</param>
        /// <returns>if replaced (the previous value, true) otherwise (null,false)</returns>
        public async Task<(V PrevValue, Boolean Replaced)> Replace(K key, V value, ExpirationTime lifespan = null, ExpirationTime maxidle = null)
        {
            var result = await Cluster.Replace(KeyMarshaller, ValueMarshaller, this, key, value, lifespan, maxidle);
            _nearCache?.Invalidate(key);
            return result;
        }
        /// <summary>
        /// Replace the value of an entry with the given version
        /// </summary>
        /// <param name="key">entry key</param>
        /// <param name="value">new value</param>
        /// <param name="version">entry version</param>
        /// <param name="lifeSpan">lifespan for the entry</param>
        /// <param name="maxIdle">max idle time</param>
        /// <returns>if replaced true otherwise false</returns>
        public async Task<Boolean> ReplaceWithVersion(K key, V value, Int64 version, ExpirationTime lifeSpan = null, ExpirationTime maxIdle = null)
        {
            var replaced = await Cluster.ReplaceWithVersion(KeyMarshaller, ValueMarshaller, (CacheBase)this, key, value, version, lifeSpan, maxIdle);
            if (replaced) _nearCache?.Invalidate(key);
            return replaced;
        }
        /// <summary>
        /// Remove an entry with the given version
        /// </summary>
        /// <param name="key">entry key</param>
        /// <param name="version">entry version</param>
        /// <returns>if replaced (the previous value, true) otherwise (null,false)</returns>
        public async Task<(V V, Boolean Removed)> RemoveWithVersion(K key, Int64 version)
        {
            var result = await Cluster.RemoveWithVersion(KeyMarshaller, ValueMarshaller, (CacheBase)this, key, version);
            if (result.Removed) _nearCache?.Invalidate(key);
            return result;
        }
        /// <summary>
        /// Run a query on the cache
        /// </summary>
        /// <param name="query">the query request</param>
        /// <returns>the query result</returns>
        public async Task<QueryResponse> Query(QueryRequest query)
        {
            return await Cluster.Query(query, (CacheBase)this);
        }
        /// <summary>
        /// A simplified method to run query
        /// </summary>
        /// This method returns the result set as a list of cache objects if the query has no select projection,
        /// otherwise return a list of tuples
        /// <param name="query">the query string</param>
        /// <param name="namedParameters">optional named parameter bindings</param>
        /// <returns>the resultSet</returns>
        public async Task<List<Object>> Query(String query, IDictionary<string, object> namedParameters = null)
        {
            var qr = new QueryRequest();
            qr.QueryString = query;
            if (namedParameters != null)
            {
                foreach (var kv in namedParameters)
                {
                    var np = new QueryRequest.Types.NamedParameter();
                    np.Name = kv.Key;
                    np.Value = WrapParameterValue(kv.Value);
                    qr.NamedParameters.Add(np);
                }
            }
            var queryResponse = await Cluster.Query(qr, (CacheBase)this);
            List<Object> result = new List<Object>();
            if (queryResponse.ProjectionSize > 0)
            {  // Query has select
                return (List<object>)unwrapWithProjection(queryResponse);
            }
            for (int i = 0; i < queryResponse.NumResults; i++)
            {
                WrappedMessage wm = queryResponse.Results[i];

                if (wm.WrappedBytes != null)
                {
                    Object u = ValueMarshaller.Unmarshall(wm.WrappedBytes.ToByteArray());
                    result.Add(u);
                }
            }
            return result;
        }
        /// <summary>
        /// Returns the set of all the cache entry keys 
        /// </summary>
        ///
        public async Task<ISet<K>> KeySet()
        {
            return await Cluster.KeySet<K>(KeyMarshaller, (CacheBase)this);
        }
        /// <summary>
        /// Put in the cache all the entries in the map
        /// </summary>
        /// <param name="map">the map of entries to put in the cache</param>
        /// <param name="lifespan">the lifespan for all the entries</param>
        /// <param name="maxidle">the maxidle for all the entries</param>
        /// <returns></returns>
        public async Task PutAll(Dictionary<K, V> map, ExpirationTime lifespan = null, ExpirationTime maxidle = null)
        {
            await Cluster.PutAll(KeyMarshaller, ValueMarshaller, this, map, lifespan, maxidle);
            if (_nearCache != null)
            {
                foreach (var key in map.Keys)
                    _nearCache.Invalidate(key);
            }
        }
        /// <summary>
        /// Get all the entries matching the keys in the set
        /// </summary>
        /// <param name="keys">the key set</param>
        /// <returns>a map with the found entries</returns>
        public async Task<IDictionary<K, V>> GetAll(ISet<K> keys)
        {
            var result = await Cluster.GetAll(KeyMarshaller, ValueMarshaller, this, keys);
            if (_nearCache != null)
            {
                foreach (var entry in result)
                    _nearCache.Put(entry.Key, entry.Value);
            }
            return result;
        }
        /// <summary>
        /// An optimized for speed version of GetAll
        /// </summary>
        /// This splits the given getall is several getall operation each of which contains keys of a specific
        /// owner. Then all the operations are sent to the relative owner. Answers are collected and returned in a single result.
        /// This operation is not atomic and could "partially" fail.
        /// TODO Allow user to await for the result
        /// <param name="keys">the key set</param>
        /// <returns>a map with the found entries</returns>
        public IPartResult<IDictionary<K, V>> GetAllPart(ISet<K> keys)
        {
            var res = Cluster.GetAllPart(KeyMarshaller, ValueMarshaller, this, keys);
            return res != null ? new GetAllPartResult<K, V>(res) : null;
        }
        /// <summary>
        /// An optimized for speed version of GetAll
        /// </summary>
        /// This splits the given putall is several putall operation each of which contains keys of a specific
        /// owner. Then all the operations are sent to the relative owner. Answers are collected and returned in a single result.
        /// This operation is not atomic and could "partially" fail.
        /// TODO Allow user to await for the result
        /// <param name="map">the map of entries to put in the cache</param>
        /// <param name="lifespan">the lifespan for all the entries</param>
        /// <param name="maxidle">the maxidle for all the entries</param>
        /// <returns></returns>
        public IPartResult PutAllPart(IDictionary<K, V> map, ExpirationTime lifespan = null, ExpirationTime maxidle = null)
        {
            var res = Cluster.PutAllPart(KeyMarshaller, ValueMarshaller, this, map, lifespan, maxidle);
            return res != null ? new PutAllPartResult(res) : null;
        }
        /// <summary>
        /// ping operation
        /// </summary>
        /// <returns>a ping result</returns>
        public async Task<PingResult> Ping()
        {
            return await Cluster.Ping(this);
        }
        /// <summary>
        /// Add a listener for events to this cache
        /// </summary>
        /// <param name="listener">the listener</param>
        /// <param name="includeState">wether or not to return the initial cache state</param>
        /// <returns></returns>
        public async Task AddListener(IClientListener listener, bool includeState = false)
        {
            await Cluster.AddListener(this, listener, includeState);
        }
        /// <summary>
        /// Remove the listener from the cache
        /// </summary>
        /// <param name="listener">the listener</param>
        /// <returns></returns>
        public async Task RemoveListener(IClientListener listener)
        {
            await Cluster.RemoveListener(this, listener);
        }

        /// <summary>
        /// Register a continuous Ickle query. Matching entries are delivered to the returned ContinuousQuery's Events channel as raw bytes.
        /// </summary>
        /// <param name="query">Ickle query string</param>
        /// <param name="namedParams">optional named parameter bindings</param>
        /// <param name="channelSize">event channel buffer size</param>
        public ContinuousQuery ContinuousQuery(string query,
            IDictionary<string, object> namedParams = null, int channelSize = 64)
        {
            return new ContinuousQuery(Cluster, this, query, namedParams, channelSize);
        }

        /// <summary>
        /// Register a typed continuous Ickle query. Events are auto-deserialized using the cache's marshallers.
        /// </summary>
        /// <param name="query">Ickle query string</param>
        /// <param name="namedParams">optional named parameter bindings</param>
        /// <param name="channelSize">event channel buffer size</param>
        public ContinuousQuery<K, V> TypedContinuousQuery(string query,
            IDictionary<string, object> namedParams = null, int channelSize = 64)
        {
            var inner = new ContinuousQuery(Cluster, this, query, namedParams, channelSize);
            return new ContinuousQuery<K, V>(inner, KeyMarshaller, ValueMarshaller, channelSize);
        }

        /// <summary>
        /// Iterate over all entries in the cache
        /// </summary>
        /// <param name="batchSize">number of entries per server round-trip</param>
        public async IAsyncEnumerable<KeyValuePair<K, V>> RetrieveEntries(int batchSize = 1000)
        {
            var iterationId = await Cluster.IterationStart(this, batchSize, false);
            try
            {
                while (true)
                {
                    var next = await Cluster.IterationNext(this, iterationId);
                    foreach (var entry in next.Entries)
                    {
                        yield return new KeyValuePair<K, V>(
                            KeyMarshaller.Unmarshall(entry.Key),
                            ValueMarshaller.Unmarshall(entry.Value));
                    }
                    if (next.Finished) break;
                }
            }
            finally
            {
                await Cluster.IterationEnd(this, iterationId);
            }
        }

        /// <summary>
        /// Iterate over all entries with their metadata
        /// </summary>
        /// <param name="batchSize">number of entries per server round-trip</param>
        public async IAsyncEnumerable<KeyValuePair<K, ValueWithMetadata<V>>> RetrieveEntriesWithMetadata(int batchSize = 1000)
        {
            var iterationId = await Cluster.IterationStart(this, batchSize, true);
            try
            {
                while (true)
                {
                    var next = await Cluster.IterationNext(this, iterationId);
                    foreach (var entry in next.Entries)
                    {
                        var vwm = new ValueWithMetadata<V>
                        {
                            Value = ValueMarshaller.Unmarshall(entry.Value),
                            Version = entry.Version,
                            Created = entry.Created,
                            Lifespan = entry.Lifespan,
                            LastUsed = entry.LastUsed,
                            MaxIdle = entry.MaxIdle
                        };
                        yield return new KeyValuePair<K, ValueWithMetadata<V>>(
                            KeyMarshaller.Unmarshall(entry.Key), vwm);
                    }
                    if (next.Finished) break;
                }
            }
            finally
            {
                await Cluster.IterationEnd(this, iterationId);
            }
        }

        /// <summary>
        /// Return all cache entries as key-value pairs
        /// </summary>
        public IAsyncEnumerable<KeyValuePair<K, V>> EntrySet(int batchSize = 1000) => RetrieveEntries(batchSize);

        /// <summary>
        /// Return all cache values
        /// </summary>
        public async IAsyncEnumerable<V> Values(int batchSize = 1000)
        {
            await foreach (var entry in RetrieveEntries(batchSize))
                yield return entry.Value;
        }

        /// <summary>
        /// Begin a new transaction on this cache
        /// </summary>
        /// <param name="timeoutMs">transaction timeout in milliseconds</param>
        public TransactionContext<K, V> BeginTransaction(long timeoutMs = 60000)
        {
            return new TransactionContext<K, V>(this, timeoutMs);
        }

        /// <summary>
        /// Enable near caching with server-side invalidation.
        /// Caches Get results locally and invalidates on remote modifications.
        /// </summary>
        /// <param name="maxEntries">maximum number of entries in the near cache</param>
        public async Task EnableNearCache(int maxEntries = 10000)
        {
            if (_nearCache != null)
                throw new InvalidOperationException("Near cache is already enabled");
            _nearCache = new NearCache<K, V>(maxEntries);
            _nearCacheListener = new NearCacheListener<K, V>(_nearCache, KeyMarshaller);
            await AddListener(_nearCacheListener);
        }

        /// <summary>
        /// Returns near cache statistics, or null if near caching is not enabled.
        /// </summary>
        public NearCacheStats NearCacheStats => _nearCache?.GetStats();

        /// <summary>
        /// Run a typed query returning deserialized results using the cache's value marshaller.
        /// </summary>
        /// <typeparam name="T">the expected result type</typeparam>
        /// <param name="query">the query string</param>
        /// <param name="namedParameters">optional named parameter bindings</param>
        /// <returns>a list of typed results</returns>
        public async Task<List<T>> Query<T>(String query, IDictionary<string, object> namedParameters = null) where T : V
        {
            var qr = new QueryRequest();
            qr.QueryString = query;
            if (namedParameters != null)
            {
                foreach (var kv in namedParameters)
                {
                    var np = new QueryRequest.Types.NamedParameter();
                    np.Name = kv.Key;
                    np.Value = WrapParameterValue(kv.Value);
                    qr.NamedParameters.Add(np);
                }
            }
            var queryResponse = await Cluster.Query(qr, (CacheBase)this);
            var result = new List<T>();
            for (int i = 0; i < queryResponse.NumResults; i++)
            {
                WrappedMessage wm = queryResponse.Results[i];
                if (wm.WrappedBytes != null)
                {
                    var u = ValueMarshaller.Unmarshall(wm.WrappedBytes.ToByteArray());
                    if (u is T typed)
                        result.Add(typed);
                }
            }
            return result;
        }

        internal static WrappedMessage WrapParameterValue(object value)
        {
            var wm = new WrappedMessage();
            switch (value)
            {
                case string s:
                    wm.WrappedString = s;
                    break;
                case int i:
                    wm.WrappedInt32 = i;
                    break;
                case long l:
                    wm.WrappedInt64 = l;
                    break;
                case double d:
                    wm.WrappedDouble = d;
                    break;
                case float f:
                    wm.WrappedFloat = f;
                    break;
                case float[] floats:
                    return WrappedMessage.Parser.ParseFrom(
                        WrappedMessageHelper.WrapFloatArray(floats));
                case bool b:
                    wm.WrappedBool = b;
                    break;
                default:
                    wm.WrappedString = value.ToString();
                    break;
            }
            return wm;
        }

        private static List<Object> unwrapWithProjection(QueryResponse resp)
        {
            List<Object> result = new List<Object>();
            if (resp.ProjectionSize == 0)
            {
                return result;
            }
            for (int i = 0; i < resp.NumResults; i++)
            {
                Object[] projection = new Object[resp.ProjectionSize];
                for (int j = 0; j < resp.ProjectionSize; j++)
                {
                    WrappedMessage wm = resp.Results[i * resp.ProjectionSize + j];
                    switch (wm.ScalarOrMessageCase)
                    {
                        case WrappedMessage.ScalarOrMessageOneofCase.WrappedDouble:
                            projection[j] = wm.WrappedDouble;
                            break;
                        case WrappedMessage.ScalarOrMessageOneofCase.WrappedFloat:
                            projection[j] = wm.WrappedFloat;
                            break;
                        case WrappedMessage.ScalarOrMessageOneofCase.WrappedInt64:
                            projection[j] = wm.WrappedInt64;
                            break;
                        case WrappedMessage.ScalarOrMessageOneofCase.WrappedUInt64:
                            projection[j] = wm.WrappedUInt64;
                            break;
                        case WrappedMessage.ScalarOrMessageOneofCase.WrappedInt32:
                            projection[j] = wm.WrappedInt32;
                            break;
                        case WrappedMessage.ScalarOrMessageOneofCase.WrappedFixed64:
                            projection[j] = wm.WrappedFixed64;
                            break;
                        case WrappedMessage.ScalarOrMessageOneofCase.WrappedFixed32:
                            projection[j] = wm.WrappedFixed32;
                            break;
                        case WrappedMessage.ScalarOrMessageOneofCase.WrappedBool:
                            projection[j] = wm.WrappedBool;
                            break;
                        case WrappedMessage.ScalarOrMessageOneofCase.WrappedString:
                            projection[j] = wm.WrappedString;
                            break;
                        case WrappedMessage.ScalarOrMessageOneofCase.WrappedBytes:
                            projection[j] = wm.WrappedBytes;
                            break;
                        case WrappedMessage.ScalarOrMessageOneofCase.WrappedUInt32:
                            projection[j] = wm.WrappedUInt32;
                            break;
                        case WrappedMessage.ScalarOrMessageOneofCase.WrappedSFixed32:
                            projection[j] = wm.WrappedSFixed32;
                            break;
                        case WrappedMessage.ScalarOrMessageOneofCase.WrappedSFixed64:
                            projection[j] = wm.WrappedSFixed64;
                            break;
                        case WrappedMessage.ScalarOrMessageOneofCase.WrappedSInt32:
                            projection[j] = wm.WrappedSInt32;
                            break;
                        case WrappedMessage.ScalarOrMessageOneofCase.WrappedSInt64:
                            projection[j] = wm.WrappedSInt64;
                            break;
                        case WrappedMessage.ScalarOrMessageOneofCase.WrappedDescriptorFullName:
                            projection[j] = wm.WrappedDescriptorFullName;
                            break;
                        case WrappedMessage.ScalarOrMessageOneofCase.WrappedMessageBytes:
                            projection[j] = wm.WrappedMessageBytes;
                            break;
                    }
                }
                result.Add(projection);
            }
            return result;
        }
    }
    public class ValueWithVersion<V>
    {
        public V Value;
        public Int64 Version;
    }

    public class ValueWithMetadata<V> : ValueWithVersion<V>
    {
        public Int64 Created = -1;
        public Int32 Lifespan = -1;
        public Int64 LastUsed = -1;
        public Int32 MaxIdle = -1;
    }
    public class VersionedResponse<V>
    {
    }

    public class ServerStatistics
    {
        public ServerStatistics(Dictionary<string, string> stats)
        {
            this.stats = stats;
        }
        /// <summary>
        ///   Number of seconds since Hot Rod started.
        /// </summary>
        public const String TIME_SINCE_START = "timeSinceStart";

        /// <summary>
        ///   Number of entries currently in the Hot Rod server.
        /// </summary>
        public const String CURRENT_NR_OF_ENTRIES = "currentNumberOfEntries";

        /// <summary>
        ///   Number of entries stored in Hot Rod server since the server started running.
        /// </summary>
        public const String TOTAL_NR_OF_ENTRIES = "approximateEntries";

        /// <summary>
        ///   Number of put operations.
        /// </summary>
        public const String STORES = "stores";

        /// <summary>
        ///   Number of get operations.
        /// </summary>
        public const String RETRIEVALS = "retrievals";

        /// <summary>
        ///   Number of get hits.
        /// </summary>
        public const String HITS = "hits";

        /// <summary>
        ///   Number of get misses.
        /// </summary>
        public const String MISSES = "misses";

        /// <summary>
        ///   Number of removal hits.
        /// </summary>
        public const String REMOVE_HITS = "removeHits";

        /// <summary>
        ///   Number of removal misses.
        /// </summary>
        public const String REMOVE_MISSES = "removeMisses";

        /// <summary>
        ///   Retrieve the complete list of statistics and their associated value.
        /// </summary>
        public IDictionary<String, String> GetStatsMap()
        {
            return stats;
        }

        /// <summary>
        ///   Retrive the value of the specified statistic.
        /// </summary>
        ///
        /// <param name="statsName">name of the statistic to retrieve</param>
        ///
        /// <returns>the value for the specified statistic as a string or null</returns>
        public String GetStatistic(String statsName)
        {
            return stats != null ? stats[statsName] : null;
        }

        /// <summary>
        ///   Retrive the value of the specified statistic.
        /// </summary>
        ///
        /// <param name="statsName">name of the statistic to retrieve</param>
        ///
        /// <returns>the value for the specified statistic as an int or -1 if no value is available</returns>
        public int GetIntStatistic(String statsName)
        {
            String value = GetStatistic(statsName);
            return value == null ? -1 : int.Parse(value);
        }
        private IDictionary<String, String> stats;
    }
    public class PingResult
    {
        public MediaType KeyType;
        public MediaType ValueType;
        public int Version;
        public int[] Operations;

    }


    public interface IPartResult
    {
        void WaitAll();

    }
    public interface IPartResult<T> : IPartResult
    {
        T Result();
    }

    internal class GetAllPartResult<K, V> : IPartResult<IDictionary<K, V>>
    {
        internal GetAllPartResult(Task<IDictionary<K, V>>[] ts)
        {
            tasks = ts;
        }
        IDictionary<K, V> result;
        Task<IDictionary<K, V>>[] tasks;
        public IDictionary<K, V> Result()
        {
            result = new Dictionary<K, V>();
            foreach (var t in tasks)
            {
                foreach (var entry in t.Result)
                {
                    result.Add(entry.Key, entry.Value);
                }
            }
            return result;
        }
        public void WaitAll()
        {
            Task.WaitAll(tasks);
        }
    }
    internal class PutAllPartResult : IPartResult
    {
        internal PutAllPartResult(Task[] ts)
        {
            tasks = ts;
        }
        Task[] tasks;
        public void WaitAll()
        {
            Task.WaitAll(tasks);
        }
    }
    public interface IClientListener
    {
        String ListenerID { get; set; }
        void OnEvent(Event e);
        void OnError(Exception ex = null);
    }
    public enum EventType : byte
    {
        CREATED = 0x60,
        MODIFIED = 0x61,
        REMOVED = 0x62,
        EXPIRED = 0x63
    }
    public class Event
    {
        public byte[] Key;
        public byte[] customData;
        public byte CustomMarker;
        public byte Retried;
        public long Version;
        public EventType Type;
        public String ListenerID;

    }

    public abstract class AbstractClientListener : IClientListener
    {
        private readonly TaskCompletionSource _completionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public abstract string ListenerID { get; set; }
        public void Wait()
        {
            try { _completionSource.Task.Wait(); }
            catch { }
        }
        internal void Activate() { }
        internal void Complete() => _completionSource.TrySetResult();
        public abstract void OnError(Exception ex = null);
        public abstract void OnEvent(Event e);
    }
}
