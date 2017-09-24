namespace Infinispan.HotRod
{
    /// <summary>
    ///   Defines all the flags available in the Hot Rod client that can influence the behavior of operations.
    /// </summary>
    public enum Flags : int
    {
        /// <summary>
        ///   By default previously existing values are not returned by map operations. This flag forces the
        /// values to be returned.
        /// </summary>
        FORCE_RETURN_VALUE = 0x01,

        /// <summary>
        ///   This flag can either be used as a request flag during a put operation to mean that the default
        /// server lifespan should be applied or as a response flag meaning that the return entry has a
        /// default lifespan value.
        /// </summary>
        DEFAULT_LIFESPAN = 0x02,

        /// <summary>
        ///   This flag can either be used as a request flag during a put operation to mean that the default
        /// server maxIdle should be applied or as a response flag meaning that the return entry has a
        /// default maxIdle value.
        /// </summary>
        DEFAULT_MAXIDLE = 0x04
    }
}