namespace Infinispan.HotRod.Config
{
    /// <summary>
    ///   Used to hold connection pool specific configurations.
    /// </summary>
    public class ConnectionPoolConfiguration
    {
        private Infinispan.HotRod.SWIG.ConnectionPoolConfiguration config;

        internal ConnectionPoolConfiguration(Infinispan.HotRod.SWIG.ConnectionPoolConfiguration config)
        {
            this.config = config;
        }

        internal Infinispan.HotRod.SWIG.ConnectionPoolConfiguration Config()
        {
            return config;
        }

        /// <summary>
        ///   Retrieves the configured exhaust action.
        /// </summary>
        public ExhaustedAction ExhaustedAction()
        {
            return config.ExhaustedAction();
        }

        /// <summary>
        ///   Retrieves the state of lifo flag.
        /// </summary>
        public bool Lifo()
        {
            return config.isLifo();
        }
        
        /// <summary>
        ///   Retrieves the configured max active value.
        /// </summary>
        public int MaxActive()
        {
            return config.getMaxActive();
        }
    
        /// <summary>
        ///   Retrieves the configured max total value.
        /// </summary>
        public int MaxTotal()
        {
            return config.getMaxTotal();
        }
    
        /// <summary>
        ///   Retrieves the configured max wait value.
        /// </summary>
        public int MaxWait()
        {
            return config.getMaxWait();
        }
    
        /// <summary>
        ///   Retrieves the configured max idle value.
        /// </summary>
        public int MaxIdle()
        {
            return config.getMaxIdle();
        }
 
        /// <summary>
        ///   Retrieves the configured min idle value.
        /// </summary>   
        public int MinIdle()
        {
            return config.getMinIdle();
        }

        /// <summary>
        ///   Retrieves the configured num test per eviction value.
        /// </summary>    
        public int NumTestsPerEvictionRun()
        {
            return config.getNumTestsPerEvictionRun();
        }

        /// <summary>
        ///   Retrieves the configured time between evictions value.
        /// </summary>    
        public int TimeBetweenEvictionRuns()
        {
            return config.getTimeBetweenEvictionRuns();
        }
   
        /// <summary>
        ///   Retrieves the configured min evictable idle time value.
        /// </summary> 
        public int MinEvictableIdleTime()
        {
            return config.getMinEvictableIdleTime();
        }

        /// <summary>
        ///   Retrieves the state of the configured test on borrow flag.
        /// </summary>
        public bool TestOnBorrow()
        {
            return config.isTestOnBorrow();
        }

        /// <summary>
        ///   Retrieves the state of the configured test on run flag.
        /// </summary>
        public bool TestOnReturn()
        {
            return config.isTestOnReturn();
        }
    
        /// <summary>
        ///   Retrieves the state of the configured test while idle flag.
        /// </summary>
        public bool TestWhileIdle()
        {
            return config.isTestWhileIdle();
        }
    }
}