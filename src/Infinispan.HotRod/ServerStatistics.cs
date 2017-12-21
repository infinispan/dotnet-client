using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinispan.HotRod
{

    /// <summary>
    ///   Defines the possible list of statistics defined by the Hot Rod server.
    /// </summary>
    public abstract class ServerStatistics
    {
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
        public const String TOTAL_NR_OF_ENTRIES = "totalNumberOfEntries";

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
        public abstract IDictionary<String, String> GetStatsMap();

        /// <summary>
        ///   Retrive the value of the specified statistic.
        /// </summary>
        ///
        /// <param name="statName">name of the statistic to retrieve</param>
        ///
        /// <returns>the value for the specified statistic as a string or null</returns>
        public abstract String GetStatistic(String statName);

        /// <summary>
        ///   Retrive the value of the specified statistic.
        /// </summary>
        ///
        /// <param name="statName">name of the statistic to retrieve</param>
        ///
        /// <returns>the value for the specified statistic as an int or -1 if no value is available</returns>
        public abstract int GetIntStatistic(String statName);
    }
}
