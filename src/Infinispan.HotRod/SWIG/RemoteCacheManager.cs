using System;

namespace Infinispan.HotRod.SWIG
{
    internal interface RemoteCacheManager
    {
        void Start();
        void Stop();
        bool IsStarted();
        RemoteByteArrayCache GetByteArrayCache();
        RemoteByteArrayCache GetByteArrayCache(String cacheName);
        RemoteByteArrayCache GetByteArrayCache(bool forceReturnValue);
        RemoteByteArrayCache GetByteArrayCache(String cacheName, bool forceReturnValue);
        bool SwitchToCluster(string clusterName);
        bool SwitchToDefaultCluster();
    }
}