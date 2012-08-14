using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinispan.DotnetClient
{
    public interface IServerStatistics
    {
        Dictionary<String, String> getStatsMap();

        String getStatistic(String statsName);

        void addStats(String name, String value);

        int getIntStatistic(String statsName);
    }
}
