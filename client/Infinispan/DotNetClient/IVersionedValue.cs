using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinispan.DotNetClient
{
    /// <summary>
    /// Besides the key and value, also contains an version. To be used in versioned operations, e.g. 
    /// RemoteCache.replaceWithVersion(Object, long)}.
    /// </summary>
    public interface IVersionedValue
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Object getValue();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        long getVersion();
    }
}
