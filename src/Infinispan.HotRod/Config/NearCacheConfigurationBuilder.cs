namespace Infinispan.HotRod.Config
{
#pragma warning disable 1591
    public class NearCacheConfigurationBuilder: AbstractConfigurationChildBuilder
    {
        private Infinispan.HotRod.SWIG.NearCacheConfigurationBuilder jniBuilder;

        internal NearCacheConfigurationBuilder(ConfigurationBuilder parentBuilder, Infinispan.HotRod.SWIG.NearCacheConfigurationBuilder jniBuilder) : base(parentBuilder)
        { 
            this.jniBuilder = jniBuilder;
        }

        public void Validate()
        {
        }

        public NearCacheConfiguration Create()
        {
            return new NearCacheConfiguration(jniBuilder.Create());
        }

        public NearCacheConfigurationBuilder Mode(NearCacheMode _mode)
        {
            jniBuilder.Mode(_mode);
            return this;
        }

        public NearCacheConfigurationBuilder MaxEntries(int _maxEntries)
        {
            jniBuilder.MaxEntries(_maxEntries);
            return this;
        }
    }
#pragma warning restore 1591
}
