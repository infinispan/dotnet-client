using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotnetClient;

namespace Infinispan.DotNetClient.Impl
{
    public class ServerStatistics : ServerStatisticsTypes,IServerStatistics
    {
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
