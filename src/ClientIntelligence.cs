namespace Infinispan.Hotrod
{
    public enum ClientIntelligence : byte
    {
        Basic = 0x01,
        TopologyAware = 0x02,
        HashDistributionAware = 0x03
    }
}
