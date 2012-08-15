using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinispan.DotnetClient
{
    /// <summary>
    /// Defines all the flags available in the Hot Rod client that can influence the behavior of operations.
    /// </summary>
    public sealed class Flag
    {
        /// <summary>
        /// By default, previously existing values for operations are not returned. E.g. RemoteCache.put(Object, Object)
        ///does <i>not</i> return the previous value associated with the key.
        ///By applying this flag, this default behavior is overridden for the scope of a single invocation, and the previous
        ///existing value is returned.
        /// </summary>
        public const byte FORCE_RETURN_VALUE = 0x0001;
        private int flagInt;

        public Flag(int flagInt)
        {
            this.flagInt = flagInt;
        }

        public int GetFlagInt()
        {
            return flagInt;
        }
    }
}
