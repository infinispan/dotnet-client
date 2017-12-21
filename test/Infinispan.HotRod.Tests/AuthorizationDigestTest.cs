namespace Infinispan.HotRod.Tests
{
    class AuthorizationDigestTest : BaseAuthorizationTest
    {
        public override string GetMech()
        {
            return "DIGEST-MD5";
        }
    }
}