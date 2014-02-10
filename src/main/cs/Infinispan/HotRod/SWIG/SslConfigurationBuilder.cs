namespace Infinispan.HotRod.SWIG
{
    internal interface SslConfigurationBuilder
    {
        SslConfiguration Create();
        void Read(SslConfiguration bean);
    }
}