using NUnit.Framework;
using System;

namespace Infinispan.HotRod.Tests.Util
{
    class TimeUtils
    {
        /*
         * Waits for a condition to be satisfied.
         * Accepts a function without parameters, returning bool value.
         */
        public static void WaitFor(Func<bool> condition, long waitTime = 1000, int pollInterval = 100)
        {
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                while (stopWatch.ElapsedMilliseconds <= waitTime)
                {
                    if (condition()) return;
                    System.Threading.Thread.Sleep(pollInterval);
                }
            }
            finally
            {
                stopWatch.Stop();
                Assert.IsTrue(condition());
            }
        }
    }
}
