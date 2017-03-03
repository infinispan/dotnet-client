using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinispan.HotRod.Tests
{
    class AuthDigestTest : BaseAuthorizationTest
    {
        public override string GetMech()
        {
            return "DIGEST-MD5";
        }
    }
}