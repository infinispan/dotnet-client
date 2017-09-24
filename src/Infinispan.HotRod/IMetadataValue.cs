namespace Infinispan.HotRod {
    
    /// <summary>
    ///   Besides the value, also contains a version and expiration information. Time values are server time representations.
    /// </summary>
    public interface IMetadataValue<V> : IVersionedValue<V> {

        /// <summary>
        ///   Time when entry was created. -1 for immortal entries.
        /// </summary>
        long GetCreated();

        /// <summary>
        ///   Lifespan of the entry in seconds. Negative values are interpreted as unlimited lifespan.
        /// </summary>
        int GetLifespan();

        /// <summary>
        ///   Time when entry was last used. -1 for immortal entries.
        /// </summary>
        long GetLastUsed();

        /// <summary>
        ///   The maximum amount of time (in seconds) this key is allowed to be idle for before it is considered as expired.
        /// </summary>
        int GetMaxIdle();
    }
}