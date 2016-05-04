using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.Infinispan.Query.Remote.Client;

namespace Infinispan.HotRod
{
    /// <summary>
    ///   Provides remote reference to a Hot Rod server/cluster.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     <b>Concurrency</b>: implementors of this interface will support multi-threaded access.
    ///   </para>
    ///   <para>
    ///     <b>Return values</b>: previously existing values for certain operations are not returned, null is returned instead.
    ///   </para>
    ///   <para>
    ///     <b>Synthetic operations</b>: aggregate operations are being implemented based on other Hot Rod operations. E.g.  GetBulk is implemented through multiple individual gets. This means that the these operations are not atomic and that they are costly since the number of network round-trips is not one.
    ///   </para>
    ///   <para>
    ///     <b>changing default behavior through Flags</b>: it is possible to change the default cache behaviour by using flags.
    ///     <code>
    ///        IRemoteCache cache = getRemoteCache(true); //passing true will enable Force Return Values
    ///        Object oldValue = cache.Put(aKey, aValue);
    ///     </code>
    ///     In the previous example, using ForceRetunValue will make the client to also return previously existing value associated with "aKey". If this flag would not be present, Infinispan would return (by default) null. This is in order to avoid fetching a possibly large object from the remote server, which might not be needed.
    ///   </para>
    ///   <para>
    ///     <b><a href="http://community.jboss.org/wiki/Eviction">Eviction and expiration</a></b>: Unlike local cache, which allows specifying time values with any granularity, HotRod only supports seconds as time units. When using .NET Hotrod Client library, you can use milliseconds also but it will result in loss of precision for values specified as nanos or milliseconds. Another fundamental difference is in the case of lifespan (naturally does NOT apply for max idle): If number of seconds is bigger than 30 days, this number of seconds is treated as UNIX time and so, represents the number of seconds since 1/1/1970. Passing 0 as maxIdle and lifeSpan (which are also default values) these values are set to infinity.
    ///   </para>
    /// </remarks>
    public interface IRemoteCache<K, V>
    {

        /// <summary>
        ///   Retrieves the name of the cache.
        /// </summary>
        /// <returns>the name of the cache</returns>
        string GetName();

        /// <summary>
        ///   Retrieves the Infinispan version.
        /// </summary>
        /// <returns>a version string</returns>
        string GetVersion();

        /// <summary>
        ///   Returns the HotRod protocol version supported by this RemoteCache implementation.
        /// </summary>
        /// <returns>a protocol version string</returns>
        string GetProtocolVersion();

        /// <summary>
        ///   Retrieves the number of records of the cache.
        /// </summary>
        ulong Size();


        /// <summary>
        ///   Used to check whether the cache is empty or not.
        /// </summary>
        /// <returns>true if number of entries of the cache is zero</returns>
        bool IsEmpty();

        /// <summary>
        ///   Used to retreive statistical information about the remote cache.
        /// </summary>
        /// <returns>the server statistics</returns>
        ServerStatistics Stats();

        /// <summary>
        ///   Insert a new entry into the cache with the specified lifetime.
        /// </summary>
        ///     
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        /// <param name="lifespan">lifespan in the specified time unit</param>
        /// <param name="lifespanUnit">the unit of time for lifespan</param>
        ///
        /// <returns>the value being replaced, or null if nothing is being replaced</returns>
        V Put(K key, V val, ulong lifespan, TimeUnit lifespanUnit);

        /// <summary>
        ///   Insert a new entry into the cache with the specified lifetime and max idle time.
        /// </summary>
        ///     
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        /// <param name="lifespan">lifespan in the specified time unit</param>
        /// <param name="lifespanUnit">the unit of time for lifespan</param>
        /// <param name="maxIdleTime">maximum idle time in in the specified time unit</param>
        /// <param name="maxIdleUnit">the unit of time for maximum idle time</param>
        ///
        /// <returns>the value being replaced, or null if nothing is being replaced</returns>
        V Put(K key, V val, ulong lifespan, TimeUnit lifespanUnit, ulong maxIdleTime, TimeUnit maxIdleUnit);

        /// <summary>
        ///   Insert a new entry into the cache.
        /// </summary>
        ///     
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        ///
        /// <returns>the value being replaced, or null if nothing is being replaced</returns>
        V Put(K key, V val);

        /// <summary>
        ///   If the specified key is absent inserts a new entry into the cache with the specified lifetime
        ///   and max idle time.
        /// </summary>
        ///     
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        /// <param name="lifespan">lifespan in the specified time unit</param>
        /// <param name="lifespanUnit">the unit of time for lifespan</param>
        /// <param name="maxIdleTime">maximum idle time in in the specified time unit</param>
        /// <param name="maxIdleUnit">the unit of time for maximum idle time</param>
        ///
        /// <returns>the value being replaced, or null if nothing is being replaced</returns>
        V PutIfAbsent(K key, V val, ulong lifespan, TimeUnit lifespanUnit, ulong maxIdleTime, TimeUnit maxIdleUnit);

        /// <summary>
        ///   If the specified key is absent inserts a new entry into the cache with the specified lifetime.
        /// </summary>
        ///     
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        /// <param name="lifespan">lifespan in the specified time unit</param>
        /// <param name="lifespanUnit">the unit of time for lifespan</param>
        ///
        /// <returns>the value being replaced, or null if nothing is being replaced</returns>
        V PutIfAbsent(K key, V val, ulong lifespan, TimeUnit lifespanUnit);

        /// <summary>
        ///   If the specified key is absent inserts a new entry into the cache.
        /// </summary>
        ///     
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        ///
        /// <returns>the value being replaced, or null if nothing is being replaced</returns>
        V PutIfAbsent(K key, V val);


        /// <summary>
        ///   Puts all the entries in the given map in the cache and for each entry sets lifespan and maxidle to
        ///   the values provided.
        /// </summary>
        ///     
        /// <param name="map">the entries to add</param>
        /// <param name="lifespan">lifespan in the specified time unit</param>
        /// <param name="lifespanUnit">the unit of time for lifespan</param>
        /// <param name="maxIdleTime">maximum idle time in in the specified time unit</param>
        /// <param name="maxIdleUnit">the unit of time for maximum idle time</param>
        void PutAll(IDictionary<K, V> map, ulong lifespan, TimeUnit lifespanUnit, ulong maxIdleTime, TimeUnit maxIdleUnit);

        /// <summary>
        ///   Puts all the entries in the given map in the cache and for each entry sets lifespan to
        ///   the value provided.
        /// </summary>
        ///
        /// <param name="map">the entries to add</param>
        /// <param name="lifespan">lifespan in the specified time unit</param>
        /// <param name="lifespanUnit">the unit of time for lifespan</param>
        void PutAll(IDictionary<K, V> map, ulong lifespan, TimeUnit lifespanUnit);

        /// <summary>
        ///   Puts all the entries in the given map in the cache.
        /// </summary>
        ///
        /// <param name="map">the entries to add</param>
        void PutAll(IDictionary<K, V> map);

        /// <summary>
        ///   Replaces an existing value with a new one with the specified lifetime and max idle time.
        /// </summary>
        ///
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        /// <param name="lifespan">lifespan in the specified time unit</param>
        /// <param name="lifespanUnit">the unit of time for lifespan</param>
        /// <param name="maxIdleTime">maximum idle time in in the specified time unit</param>
        /// <param name="maxIdleUnit">the unit of time for maximum idle time</param>
        ///
        /// <returns>the value being replaced, or null if nothing is being replaced</returns>
        V Replace(K key, V val, ulong lifespan, TimeUnit lifespanUnit, ulong maxIdleTime, TimeUnit maxIdleUnit);

        /// <summary>
        ///   Replaces an existing value with a new one with the specified lifetime.
        /// </summary>
        ///
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        /// <param name="lifespan">lifespan in the specified time unit</param>
        /// <param name="lifespanUnit">the unit of time for lifespan</param>
        ///
        /// <returns>the value being replaced, or null if nothing is being replaced</returns>
        V Replace(K key, V val, ulong lifespan, TimeUnit lifespanUnit);

        /// <summary>
        ///   Replaces an existing value with a new value.
        /// </summary>
        ///
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        ///
        /// <returns>the value being replaced, or null if nothing is being replaced</returns>
        V Replace(K key, V val);

        /// <summary>
        ///   Checks whether the passes key exists in the cache.
        /// </summary>
        ///
        /// <param name="key">key</param>
        ///
        /// <returns>true if the passed key exists and false if it doesn't.</returns>
        bool ContainsKey(K key);

        /// <summary>
        ///   Checks whether the passes value exists in the cache.
        /// </summary>
        ///
        /// <param name="val">value</param>
        ///
        /// <returns>true if the passed value exists and false if it doesn't.</returns>
        bool ContainsValue(V val);

        /// <summary>
        ///   Retrieve from the cache the value associated with the given key.
        /// </summary>
        ///
        /// <param name="key">key</param>
        ///
        /// <returns>the value associated with the key or null</returns>
        V Get(K key);

        /// <summary>
        ///   Bulk get operations, returns the entries within the remote cache limiting the returned
        ///   set of values to the specified size. No ordering is guaranteed, and there is no guarantee
        ///   that "size" elements are returned (e.g. if the number of elements in the back-end server
        ///   is smaller that "size")
        /// </summary>
        ///
        /// <param name="size">limit on the number of elements to be returned</param>
        ///
        /// <returns>dictionary with retrieved data</returns>
        IDictionary<K, V> GetBulk(int size);

        /// <summary>
        ///   Bulk get operations, returns all the entries within the remote cache.
        /// </summary>
        ///
        /// <returns>dictionary with retrieved data</returns>
        IDictionary<K, V> GetBulk();

        /// <summary>
        ///  Removes from the cache the value associated with the given key. 
        /// </summary>
        ///
        /// <param name="key">Key</param>
        ///
        /// <returns>the removed value or null</returns>
        V Remove(K key);

        /// <summary>
        ///   Clears the cache.
        /// </summary>
        void Clear();

        /// <summary>
        ///   Returns the VersionedValue associated to the supplied key param, or null if it doesn't exist.
        /// </summary>
        ///
        /// <param name="key"></param>
        ///
        /// <returns>IVersionedValue associated to the supplied key param, or null if it doesn't exist.</returns>
        IVersionedValue<V> GetVersioned(K key);

        /// <summary>
        ///   Returns the MetadataValue associated to the supplied key param, or null if it doesn't exist.
        /// </summary>
        ///
        /// <param name="key">key</param>
        ///
        /// <returns>IMetadataValue associated to the supplied key param, or null if it doesn't exist.</returns>
        IMetadataValue<V> GetWithMetadata(K key);

        /// <summary>
        ///   Removes the given value only if its version matches the supplied version.
        /// </summary>
        ///
        /// <param name="key">key</param>
        /// <param name="version">version numeric version that should match the one in the server for the operation to succeed</param>
        ///
        /// <returns>true if the entry was removed and false otherwise</returns>
        bool RemoveWithVersion(K key, ulong version);

        /// <summary>
        /// Replaces the given value only if its version matches the supplied version.
        /// </summary>
        ///
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        /// <param name="version">version numeric version that should match the one in the server for the operation to succeed</param>
        /// <param name="lifespan">the lifespan of the new entry</param>
        /// <param name="maxIdleTime">the max idle time of the new entry</param>
        ///
        /// <returns>true if the value has been replaced and false otherwise</returns>
        bool ReplaceWithVersion(K key, V val, ulong version, ulong lifespan, ulong maxIdleTime);

        /// <summary>
        /// Replaces the given value only if its version matches the supplied version.
        /// </summary>
        ///
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        /// <param name="version">version numeric version that should match the one in the server for the operation to succeed</param>
        /// <param name="lifespan">the lifespan of the new entry</param>
        ///
        /// <returns>true if the value has been replaced and false otherwise</returns>
        bool ReplaceWithVersion(K key, V val, ulong version, ulong lifespan);

        /// <summary>
        /// Replaces the given value only if its version matches the supplied version.
        /// </summary>
        ///
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        /// <param name="version">version numeric version that should match the one in the server for the operation to succeed</param>
        ///
        /// <returns>true if the value has been replaced and false otherwise</returns>
        bool ReplaceWithVersion(K key, V val, ulong version);

        /// <summary>
        ///  Returns the set of entries present in the cache.
        /// </summary>
        ///
        /// <returns>a set with the key/value pairs present in the cache</returns>
        ISet<KeyValuePair<K, V>> EntrySet();

        /// <summary>
        ///  Returns the set of keys present in the cache.
        /// </summary>
        ///
        /// <returns>a set with the keys present in the cache</returns>
        ISet<K> KeySet();

        /// <summary>
        ///  Returns the set of values present in the cache.
        /// </summary>
        ///
        /// <returns>a set with the values present in the cache</returns>
        IList<V> Values();

        /// <summary>
        ///   Applies one or more flags to the scope of a single invocation.
        /// </summary>
        ///
        /// <returns>the current IRemoteCache instance to continue running operations on</returns>
        ///
        /// <example>
        ///   remoteCache.WithFlags(Flags.FORCE_RETURN_VALUE | Flags.DEFAULT_LIFETIME).put("hello", "world");
        /// </example>
        IRemoteCache<K, V> WithFlags(Flags flags);

        /// <summary>
        ///   Query the cache
        /// </summary>
        ///
        /// <param name="query">the query sring</param>
        ///
        /// <returns>A query response containing the result set</returns>
        ///
        /// <example>
        ///   QueryRequest qReq = new QueryResponse();
        ///   qReq.JpqlString="from sample_bank_account.User";
        ///   QueryResponse qRes= remoteCache.query(qReq);
        /// </example>
        QueryResponse Query(QueryRequest query);

        /// <summary>
        ///   Execute a script on the server
        /// </summary>
        /// <param name="scriptName">script name</param>
        /// <param name="dict">map of the arguments</param>
        /// <returns>a stream of byte containing the result</returns>
        byte[] Execute(String scriptName, IDictionary<String, String> dict);
    }

}
