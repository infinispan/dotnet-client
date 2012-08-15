using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinispan.DotNetClient
{
    public sealed class ServerStatisticsTypes
    {
        ///<summary>
        ///Number of seconds since Hot Rod started.
        ///</summary>
        public static readonly String TIME_SINCE_START = "timeSinceStart";

        ///<summary>
        ///Number of entries currently in the Hot Rod server
        ///</summary>
        public static readonly String CURRENT_NR_OF_ENTRIES = "currentNumberOfEntries";

        ///<summary>
        ///Total Number of entries stored in Hot Rod server.
        ///</summary>
        public static readonly String TOTAL_NR_OF_ENTRIES = "totalNumberOfEntries";

        ///<summary>
        ///Number of put operations.
        ///</summary>
        public static readonly String STORES = "stores";

        ///<summary>
        ///Number of get operations.
        ///</summary>
        public static readonly String RETRIEVALS = "retrievals";

        ///<summary>
        ///Number of get hits.
        ///</summary>
        public static readonly String HITS = "hits";

        ///<summary>
        ///Number of get misses.
        ///</summary>
        public static readonly String MISSES = "misses";

        ///<summary>
        ///Number of removal hits.
        ///</summary>
        public static readonly String REMOVE_HITS = "removeHits";

        ///<summary>
        ///Number of removal misses.
        ///</summary>
        public static readonly String REMOVE_MISSES = "removeMisses";
    }
}
