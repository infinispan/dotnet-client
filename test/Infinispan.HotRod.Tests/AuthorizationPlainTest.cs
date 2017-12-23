using NUnit.Framework;
namespace Infinispan.HotRod.Tests.ClusteredSaslCsXml2
{
    [TestFixture]
    [Category("clustered_sasl_cs_xml_2")]
    public class AuthorizationPlainTest : BaseAuthorizationTest
    {
        public override string GetMech()
        {
            return "PLAIN";
        }
    }
}
