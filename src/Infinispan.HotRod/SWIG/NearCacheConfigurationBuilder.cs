namespace Infinispan.HotRod.SWIG {
using Infinispan.HotRod;
    internal interface NearCacheConfigurationBuilder
    {
          NearCacheConfiguration Create();
          NearCacheConfigurationBuilder Mode(NearCacheMode mode);
          NearCacheMode GetMode();
          NearCacheConfigurationBuilder MaxEntries(int maxEntries);
          int GetMaxEntries();
    }
}
