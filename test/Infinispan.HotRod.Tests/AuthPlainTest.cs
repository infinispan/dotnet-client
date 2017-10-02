using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Infinispan.HotRod.Tests
{
    [TestFixture]
    class AuthPlainTest : BaseAuthorizationTest
    {
        public override string GetMech()
        {
            return "PLAIN";
        }
    }
}
