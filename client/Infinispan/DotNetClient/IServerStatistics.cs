using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinispan.DotnetClient
{
    public interface IServerStatistics
    {
        Dictionary<String, String> GetStatsMap();

        String GetStatistic(String statsName);

        void AddStats(String name, String value);

        int GetIntStatistic(String statsName);
    }
}
