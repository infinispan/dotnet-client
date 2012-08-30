using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinispan.DotNetClient
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

        /// <summary>
        /// New Flag can be instantiated by calling this method.
        /// </summary>
        /// <param name="flagInt">Value of the flag</param>
        public Flag(int flagInt)
        {
            this.flagInt = flagInt;
        }

        /// <summary>
        /// When we need to get the value associated with a certain flag, this method can be used.
        /// </summary>
        /// <returns>Integer Value associated with the flag</returns>
        public int GetFlagInt()
        {
            return flagInt;
        }
    }
}
