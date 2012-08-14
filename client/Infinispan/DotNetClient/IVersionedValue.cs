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
        /// Get the value of the cache entry retrieved for the given key and version.
        /// </summary>
        /// <returns>Value of the cache entry retrieved</returns>
        Object GetValue();
        /// <summary>
        /// Cache entries have a version number which indicates the current version of the data of the cache entry.
        /// </summary>
        /// <returns>Version number of the cache entry retrieved</returns>
        long GetVersion();
    }
}
