using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinispan.DotNetClient.Hotrod
{
    public class ServerStatistics
    {

        /**
        * Number of seconds since Hot Rod started.
        */
        public const string TIME_SINCE_START = "timeSinceStart";

        /**
         * Number of entries currently in the Hot Rod server
         */
        public const string CURRENT_NR_OF_ENTRIES = "currentNumberOfEntries";

        /**
         * Number of entries stored in Hot Rod server.
         */
        public const string TOTAL_NR_OF_ENTRIES = "totalNumberOfEntries";

        /**
         * Number of put operations.
         */
        public const string STORES = "stores";

        /**
         * Number of get operations.
         */
        public const string RETRIEVALS = "retrievals";

        /**
         * Number of get hits.
         */
        public const string HITS = "hits";

        /**
         * Number of get misses.
         */
        public const string MISSES = "misses";

        /**
         * Number of removal hits.
         */
        public const string REMOVE_HITS = "removeHits";

        /**
         * Number of removal misses.
         */
        public const string REMOVE_MISSES = "removeMisses";

        private Dictionary<String, String> stats = new Dictionary<String, String>();

        public Dictionary<String, String> getStatsMap()
        {
            return stats;
        }

        public String getStatistic(String statsName)
        {
            return stats[statsName];
        }

        public void addStats(String name, String value)
        {
            stats.Add(name, value);
        }

        public int getIntStatistic(String statsName)
        {
            String value = stats[statsName];
            return value == null ? int.MinValue : int.Parse(value); //If null, MINVALUE will be returned
        }
    }
}
