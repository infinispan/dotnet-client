namespace Infinispan.HotRod.SWIG
{
    internal interface SslConfigurationBuilder
    {
        SslConfiguration Create();
        void Read(SslConfiguration bean);
        SslConfigurationBuilder Enable();
        SslConfigurationBuilder ClientCertificateFile(string filename);
        SslConfigurationBuilder ServerCAFile(string filename);

    }
}
