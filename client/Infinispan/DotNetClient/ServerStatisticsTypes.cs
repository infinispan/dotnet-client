using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinispan.DotnetClient
{
    public abstract class ServerStatisticsTypes
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
    }
}
