using NUnit.Framework;
namespace Infinispan.HotRod.Tests.ClusteredSaslCsXml2
{
    [TestFixture]
    [Category("clustered_sasl_cs_xml_2")]
    [Category("AuthenticationTestSuite")]
    public class AuthorizationDigestTest : BaseAuthorizationTest
    {
        public override string GetMech()
        {
            return "DIGEST-MD5";
        }
    }
}
