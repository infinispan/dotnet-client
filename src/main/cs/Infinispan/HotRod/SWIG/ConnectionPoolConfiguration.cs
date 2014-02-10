namespace Infinispan.HotRod.SWIG
{
    internal interface ConnectionPoolConfiguration
    {
        bool isLifo();
        int getMaxActive();
        int getMaxTotal();
        int getMaxWait();
        int getMaxIdle();
        int getMinIdle();
        int getNumTestsPerEvictionRun();
        int getTimeBetweenEvictionRuns();
        int getMinEvictableIdleTime();
        bool isTestOnBorrow();
        bool isTestOnReturn();
        bool isTestWhileIdle();

        Infinispan.HotRod.Config.ExhaustedAction ExhaustedAction();
    }
}