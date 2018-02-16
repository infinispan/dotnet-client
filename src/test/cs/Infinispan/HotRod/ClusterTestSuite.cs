using Infinispan.HotRod.Tests;
using Infinispan.HotRod.Tests.Util;
using NUnit.Framework;
using System;
using System.Collections;

namespace Infinispan.HotRod.TestSuites
{
    // This class enlist all the cluster related tests.
    // These test do not have a common setup/shutdown procedure
    // so nothing more to do here
    public class ClusterTestSuite
    {
        [Suite]
        public static IEnumerable Suite
        {
            get
            {
                var suite = new ArrayList();
                suite.Add(new NearCacheFailoverTest());
                return suite;
            }
        }
    }
}
