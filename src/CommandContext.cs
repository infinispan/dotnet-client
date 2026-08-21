using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;


namespace Infinispan.Hotrod
{
    internal class CommandContext
    {
        public MediaType CmdReqMediaType;
        public MediaType CmdResMediaType;
        public bool IsReqResCommand;
        public InfinispanConnection Client;
        public CacheBase Cache;
        public byte[] CacheNameAsBytes { get { return (Cache != null) ? Cache.NameAsBytes : new byte[] { }; } }
        public long MessageId;
        public byte ClientIntelligence { get { return Client.Host.Cluster.ClientIntelligence; } }
        public byte? VersionOverride;
        public byte Version => VersionOverride ?? Client.Host.Cluster.Version;
        public UInt32 TopologyId { get { return Client.Host.Cluster.TopologyId; } }
        public bool IsVersion40OrAbove => Version >= 40;
    }
}
