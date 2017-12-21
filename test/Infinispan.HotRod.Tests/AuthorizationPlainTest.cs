namespace Infinispan.HotRod.Tests
{
    class AuthorizationPlainTest : BaseAuthorizationTest
    {
        public override string GetMech()
        {
            return "PLAIN";
        }
    }
}
