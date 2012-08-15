using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotnetClient;

namespace Infinispan.DotNetClient.Impl
{
    public class ServerStatistics : IServerStatistics
    {
        private Dictionary<String, String> stats = new Dictionary<String, String>();

        public Dictionary<String, String> GetStatsMap()
        {
            return stats;
        }

        public String GetStatistic(String statsName)
        {
            return stats[statsName];
        }

        public void AddStats(String name, String value)
        {
            stats.Add(name, value);
        }

        public int GetIntStatistic(String statsName)
        {
            String value = stats[statsName];
            return value == null ? int.MinValue : int.Parse(value); //If null, MINVALUE will be returned
        }
    }
}
