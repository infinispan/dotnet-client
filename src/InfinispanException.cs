using System;
using System.Collections.Generic;
using System.Text;

namespace Infinispan.Hotrod
{
    /// <summary>
    /// InfinispanException represent a generic exception
    /// </summary>
    public class InfinispanException : Exception
    {
        /// <summary>
        /// CommandResult collect all the info related to the command execution
        /// </summary>
        public CommandResult Result;
        public InfinispanException(CommandResult result) : base(result.ErrorMessage)
        {
            Result = result;
        }
        public InfinispanException(string message) : base(message) { }
    }
}
