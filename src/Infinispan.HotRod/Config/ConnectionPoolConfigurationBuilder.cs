#pragma warning disable 1591
namespace Infinispan.HotRod.Config
{
    public class ConnectionPoolConfigurationBuilder : AbstractConfigurationChildBuilder
    {
        private Infinispan.HotRod.SWIG.ConnectionPoolConfigurationBuilder jniBuilder;

        internal ConnectionPoolConfigurationBuilder(ConfigurationBuilder parentBuilder,
                                                  Infinispan.HotRod.SWIG.ConnectionPoolConfigurationBuilder jniBuilder) : base(parentBuilder)
        {
            this.jniBuilder = jniBuilder;
        }

        public void Validate()
        {
        }

        public ConnectionPoolConfiguration Create()
        {
            return new ConnectionPoolConfiguration(jniBuilder.Create());
        }

        public ConnectionPoolConfigurationBuilder ExhaustedAction(ExhaustedAction exhaustedAction)
        {
            jniBuilder.ExhaustedAction(exhaustedAction);
            return this;
        }

        public ConnectionPoolConfigurationBuilder Lifo(bool lifo)
        {
            jniBuilder.Lifo(lifo);
            return this;
        }
        
        public ConnectionPoolConfigurationBuilder MaxActive(int maxActive)
        {
            jniBuilder.MaxActive(maxActive);
            return this;
        }
    
        public ConnectionPoolConfigurationBuilder MaxTotal(int maxTotal)
        {
            jniBuilder.MaxTotal(maxTotal);
            return this;
        }
    
        public ConnectionPoolConfigurationBuilder MaxWait(int maxWait)
        {
            jniBuilder.MaxWait(maxWait);
            return this;
        }
    
        public ConnectionPoolConfigurationBuilder MaxIdle(int maxIdle)
        {
            jniBuilder.MaxIdle(maxIdle);
            return this;
        }
    
        public ConnectionPoolConfigurationBuilder MinIdle(int minIdle)
        {
            jniBuilder.MinIdle(minIdle);
            return this;
        }
    
        public ConnectionPoolConfigurationBuilder NumTestsPerEvictionRun(int numTestsPerEvictionRun)
        {
            jniBuilder.NumTestsPerEvictionRun(numTestsPerEvictionRun);
            return this;
        }
    
        public ConnectionPoolConfigurationBuilder TimeBetweenEvictionRuns(int timeBetweenEvictionRuns)
        {
            jniBuilder.TimeBetweenEvictionRuns(timeBetweenEvictionRuns);
            return this;
        }
    
        public ConnectionPoolConfigurationBuilder MinEvictableIdleTime(int minEvictableIdleTime)
        {
            jniBuilder.MinEvictableIdleTime(minEvictableIdleTime);
            return this;
        }
    
        public ConnectionPoolConfigurationBuilder TestOnBorrow(bool testOnBorrow)
        {
            jniBuilder.TestOnBorrow(testOnBorrow);
            return this;
        }
    
        public ConnectionPoolConfigurationBuilder TestOnReturn(bool testOnReturn)
        {
            jniBuilder.TestOnReturn(testOnReturn);
            return this;
        }
    
        public ConnectionPoolConfigurationBuilder TestWhileIdle(bool testWhileIdle)
        {
            jniBuilder.TestWhileIdle(testWhileIdle);
            return this;
        }
    }
}
#pragma warning restore 1591