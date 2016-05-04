namespace Infinispan.HotRod.Config
{
#pragma warning disable 1591
    public class SslConfigurationBuilder : AbstractConfigurationChildBuilder, IBuilder<SslConfiguration>
    {
        private Infinispan.HotRod.SWIG.SslConfigurationBuilder builder;

        internal SslConfigurationBuilder(ConfigurationBuilder parent, Infinispan.HotRod.SWIG.SslConfigurationBuilder builder) : base(parent)
        {
            this.builder = builder;
        }

        public void Validate()
        {
        }

        public SslConfiguration Create()
        {
            return new SslConfiguration(builder.Create());
        }

        public SslConfigurationBuilder Enable()
        {
            builder.Enable();
            return this;
        }

        public SslConfigurationBuilder ServerCAFile(string filename)
        {
            builder.ServerCAFile(filename);
            return this;
        }

        public SslConfigurationBuilder ClientCertificateFile(string filename)
        {
            builder.ClientCertificateFile(filename);
            return this;
        }

        public IBuilder<SslConfiguration> Read(SslConfiguration bean)
        {
            builder.Read(bean.Config());
            return this;
        }

    }
#pragma warning restore 1591
}