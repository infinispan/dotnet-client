namespace Infinispan.HotRod.SWIG
{
    internal interface SslConfigurationBuilder
    {
        SslConfiguration Create();
        SslConfigurationBuilder Enable();
        SslConfigurationBuilder ClientCertificateFile(string filename);
        SslConfigurationBuilder ServerCAFile(string filename);
        SslConfigurationBuilder SniHostName(string hostName);

    }
}
