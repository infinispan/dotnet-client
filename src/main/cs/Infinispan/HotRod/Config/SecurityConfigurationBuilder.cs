
namespace Infinispan.HotRod.Config
{
#pragma warning disable 1591
    public class SecurityConfigurationBuilder: AbstractConfigurationChildBuilder
    {
        private Infinispan.HotRod.SWIG.SecurityConfigurationBuilder jniBuilder;
        private ConfigurationBuilder parentBuilder;

        internal SecurityConfigurationBuilder(ConfigurationBuilder parentBuilder, Infinispan.HotRod.SWIG.SecurityConfigurationBuilder jniBuilder) : base(parentBuilder)
        { 
            this.jniBuilder = jniBuilder;
            this.parentBuilder = parentBuilder;
        }

        public void Validate()
        {
        }

        public SecurityConfiguration Create()
        {
            return new SecurityConfiguration(jniBuilder.Create());
        }

        public AuthenticationConfigurationBuilder Authentication()
        {
            return new AuthenticationConfigurationBuilder(parentBuilder, jniBuilder.Authentication());
        }
      

    }
#pragma warning restore 1591
}
