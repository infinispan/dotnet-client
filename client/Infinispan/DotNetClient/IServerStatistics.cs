using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinispan.DotNetClient
{
    /// <summary>
    /// Defines the interface which can be used to obtain Statistical data described in Infinispan.DotNetClient.ServerStatisticsTypes
    /// </summary>
    public interface IServerStatistics
    {
        Dictionary<String, String> GetStatsMap();

        String GetStatistic(String statsName);

        void AddStats(String name, String value);

        int GetIntStatistic(String statsName);
    }
}
