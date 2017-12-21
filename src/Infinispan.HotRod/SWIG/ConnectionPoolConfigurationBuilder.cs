using Infinispan.HotRod.Config;

namespace Infinispan.HotRod.SWIG
{
    internal interface ConnectionPoolConfigurationBuilder
    {
        ConnectionPoolConfiguration Create();
        ConnectionPoolConfigurationBuilder ExhaustedAction(ExhaustedAction exhaustedAction);
        ConnectionPoolConfigurationBuilder Lifo(bool lifo);
        ConnectionPoolConfigurationBuilder MaxActive(int maxActive);
        ConnectionPoolConfigurationBuilder MaxTotal(int maxTotal);
        ConnectionPoolConfigurationBuilder MaxWait(int maxWait);
        ConnectionPoolConfigurationBuilder MaxIdle(int maxIdle);
        ConnectionPoolConfigurationBuilder MinIdle(int minIdle);
        ConnectionPoolConfigurationBuilder NumTestsPerEvictionRun(int numTestsPerEvictionRun);
        ConnectionPoolConfigurationBuilder TimeBetweenEvictionRuns(int timeBetweenEvictionRuns);
        ConnectionPoolConfigurationBuilder MinEvictableIdleTime(int minEvictableIdleTime);
        ConnectionPoolConfigurationBuilder TestOnBorrow(bool testOnBorrow);
        ConnectionPoolConfigurationBuilder TestOnReturn(bool testOnReturn);
        ConnectionPoolConfigurationBuilder TestWhileIdle(bool testWhileIdle);
    }
}