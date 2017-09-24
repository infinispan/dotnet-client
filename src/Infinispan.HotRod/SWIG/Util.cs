using System;

namespace Infinispan.HotRod.SWIG
{
    internal class Util
    {
        public static bool Use64()
        {
            return Environment.Is64BitProcess;
        }
    }
}