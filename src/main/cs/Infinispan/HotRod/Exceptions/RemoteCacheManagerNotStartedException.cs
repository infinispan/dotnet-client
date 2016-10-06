using System;

namespace Infinispan.HotRod.Exceptions
{
    /// <summary>
    ///   Used to indicate that invocations on an IRemoveCache are done before
    /// the underlying RemoveCacheManager has been started.
    /// </summary>
    public class RemoteCacheManagerNotStartedException : HotRodClientException
    {
        internal RemoteCacheManagerNotStartedException(string message) : base(message)
        {
        }
    }    
}