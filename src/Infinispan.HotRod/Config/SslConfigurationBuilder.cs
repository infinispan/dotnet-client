namespace Infinispan.HotRod.Config
{
#pragma warning disable 1591
    public class SslConfigurationBuilder: AbstractConfigurationChildBuilder
    {
        private Infinispan.HotRod.SWIG.SslConfigurationBuilder jniBuilder;

        internal SslConfigurationBuilder(ConfigurationBuilder parentBuilder, Infinispan.HotRod.SWIG.SslConfigurationBuilder jniBuilder) : base(parentBuilder)
        { 
            this.jniBuilder = jniBuilder;
        }

        public void Validate()
        {
        }

        public SslConfiguration Create()
        {
            return new SslConfiguration(jniBuilder.Create());
        }

        public SslConfigurationBuilder Enable()
        {
            jniBuilder.Enable();
            return this;
        }

        public SslConfigurationBuilder ServerCAFile(string filename)
        {
            jniBuilder.ServerCAFile(filename);
            return this;
        }

        public SslConfigurationBuilder ClientCertificateFile(string filename)
        {
            jniBuilder.ClientCertificateFile(filename);
            return this;
        }

        public SslConfigurationBuilder SniHostName(string sniHostName)
        {
            jniBuilder.SniHostName(sniHostName);
            return this;
        }

    }
#pragma warning restore 1591
}