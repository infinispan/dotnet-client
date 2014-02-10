using System;

namespace Infinispan.HotRod.Tests
{
    /**
       Utility class which can be used to generated unique
       keys when running multiple tests againts the same
       cache.
     */
    public class UniqueKey
    {
        private static ulong key = 0;

        public static string NextKey()
        {
            return String.Format("key{0}", key++);
        }
    }
}