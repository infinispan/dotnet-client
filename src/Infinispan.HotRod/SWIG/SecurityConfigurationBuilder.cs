namespace Infinispan.HotRod.SWIG

{
    internal interface SecurityConfigurationBuilder
    {
        SecurityConfiguration Create();
        AuthenticationConfigurationBuilder Authentication();
    }
}
