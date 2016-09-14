#pragma warning disable 1591
namespace Infinispan.HotRod.Config
{
    public class ConnectionPoolConfigurationBuilder
    {
        private Infinispan.HotRod.SWIG.ConnectionPoolConfigurationBuilder builder;

        internal ConnectionPoolConfigurationBuilder(ConfigurationBuilder parent,
                                                  Infinispan.HotRod.SWIG.ConnectionPoolConfigurationBuilder builder)
        {
            this.builder = builder;
        }

        public void Validate()
        {
        }

        public ConnectionPoolConfiguration Create()
        {
            return new ConnectionPoolConfiguration(builder.Create());
        }

        public ConnectionPoolConfigurationBuilder ExhaustedAction(ExhaustedAction exhaustedAction)
        {
            builder.ExhaustedAction(exhaustedAction);
            return this;
        }

        public ConnectionPoolConfigurationBuilder Lifo(bool lifo)
        {
            builder.Lifo(lifo);
            return this;
        }
        
        public ConnectionPoolConfigurationBuilder MaxActive(int maxActive)
        {
            builder.MaxActive(maxActive);
            return this;
        }
    
        public ConnectionPoolConfigurationBuilder MaxTotal(int maxTotal)
        {
            builder.MaxTotal(maxTotal);
            return this;
        }
    
        public ConnectionPoolConfigurationBuilder MaxWait(int maxWait)
        {
            builder.MaxWait(maxWait);
            return this;
        }
    
        public ConnectionPoolConfigurationBuilder MaxIdle(int maxIdle)
        {
            builder.MaxIdle(maxIdle);
            return this;
        }
    
        public ConnectionPoolConfigurationBuilder MinIdle(int minIdle)
        {
            builder.MinIdle(minIdle);
            return this;
        }
    
        public ConnectionPoolConfigurationBuilder NumTestsPerEvictionRun(int numTestsPerEvictionRun)
        {
            builder.NumTestsPerEvictionRun(numTestsPerEvictionRun);
            return this;
        }
    
        public ConnectionPoolConfigurationBuilder TimeBetweenEvictionRuns(int timeBetweenEvictionRuns)
        {
            builder.TimeBetweenEvictionRuns(timeBetweenEvictionRuns);
            return this;
        }
    
        public ConnectionPoolConfigurationBuilder MinEvictableIdleTime(int minEvictableIdleTime)
        {
            builder.MinEvictableIdleTime(minEvictableIdleTime);
            return this;
        }
    
        public ConnectionPoolConfigurationBuilder TestOnBorrow(bool testOnBorrow)
        {
            builder.TestOnBorrow(testOnBorrow);
            return this;
        }
    
        public ConnectionPoolConfigurationBuilder TestOnReturn(bool testOnReturn)
        {
            builder.TestOnReturn(testOnReturn);
            return this;
        }
    
        public ConnectionPoolConfigurationBuilder TestWhileIdle(bool testWhileIdle)
        {
            builder.TestWhileIdle(testWhileIdle);
            return this;
        }
    }
}
#pragma warning restore 1591