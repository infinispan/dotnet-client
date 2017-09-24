namespace Infinispan.HotRod.SWIG
{
    internal interface ClusterConfigurationBuilder
    {
        ClusterConfigurationBuilder AddClusterNode(string host, int port);
    }
}