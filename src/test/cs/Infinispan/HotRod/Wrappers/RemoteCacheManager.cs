using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.HotRod.Config;
using Infinispan.HotRod.Impl;

namespace Infinispan.HotRod.Wrappers
{
    public class RemoteCacheManager
    {
        private Infinispan.HotRod.RemoteCacheManager manager;

        public RemoteCacheManager(Configuration configuration, Infinispan.HotRod.IMarshaller marshaller, bool start)
        {
            manager = new Infinispan.HotRod.RemoteCacheManager(configuration, marshaller, start);
        }

        public RemoteCacheManager(Configuration configuration, Infinispan.HotRod.IMarshaller marshaller)
        {
            manager = new Infinispan.HotRod.RemoteCacheManager(configuration, marshaller);
        }

        public RemoteCacheManager(Configuration configuration, bool start)
        {
            // Don't serialize, use the already serialized data.
            manager = new Infinispan.HotRod.RemoteCacheManager(configuration, new Infinispan.HotRod.IdentityMarshaller(), start);
        }

        public RemoteCacheManager(Configuration configuration, bool start, bool useCompatibilityMarshaller)
        {
            if (useCompatibilityMarshaller)
                manager = new Infinispan.HotRod.RemoteCacheManager(configuration, new Infinispan.HotRod.CompatibilityMarshaller(), start);
            else
                // Don't serialize, use the already serialized data.
                manager = new Infinispan.HotRod.RemoteCacheManager(configuration, new Infinispan.HotRod.IdentityMarshaller(), start);
        }

        public RemoteCacheManager(Configuration configuration)
        {
            // Don't serialize, use the already serialized data.
            manager = new Infinispan.HotRod.RemoteCacheManager(configuration, new Infinispan.HotRod.IdentityMarshaller());
        }

        public RemoteCacheManager(bool start)
        {
            // Don't serialize, use the already serialized data.
            manager = new Infinispan.HotRod.RemoteCacheManager(new Infinispan.HotRod.IdentityMarshaller(), start);
        }

        public RemoteCacheManager()
        {
            // Don't serialize, use the already serialized data.
            manager = new Infinispan.HotRod.RemoteCacheManager(new Infinispan.HotRod.IdentityMarshaller());
        }

        public void Start()
        {
            manager.Start();
        }

        public void Stop()
        {
            manager.Stop();
        }

        public bool IsStarted()
        {
            return manager.IsStarted();
        }

        public RemoteCache GetCache()
        {
            return new RemoteCache(manager.GetCache<object, object>());
        }

        public RemoteCache GetCache(String cacheName)
        {
            return new RemoteCache(manager.GetCache<object, object>(cacheName));
        }

        public RemoteCache GetCache(bool forceReturnValue)
        {
            return new RemoteCache(manager.GetCache<object, object>(forceReturnValue));
        }

        public RemoteCache GetCache(String cacheName, bool forceReturnValue)
        {
            return new RemoteCache(manager.GetCache<object, object>(cacheName, forceReturnValue));
        }
        public bool SwitchToCluster(String clusterName) {
            return manager.SwitchToCluster(clusterName);
        }
        
        public bool SwitchToDefaultCluster() {
            return manager.SwitchToDefaultCluster();
        }
    }
}
