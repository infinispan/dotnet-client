using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinispan.HotRod.Impl
{
#pragma warning disable 1591
    public class ServerStatisticsImpl : ServerStatistics
    {
        private IDictionary<String, String> stats;
        
        public ServerStatisticsImpl(IDictionary<String, String> stats)
        {
            this.stats = stats;
        }

        public override IDictionary<String, String> GetStatsMap()
        {
            return stats;
        }

        public override String GetStatistic(String statsName)
        {
            return stats != null ? stats[statsName] : null;
        }

        public override int GetIntStatistic(String statsName)
        {
            String value = GetStatistic(statsName);
            return value == null ? -1 : int.Parse(value);
        }
    }
#pragma warning restore 1591
}
