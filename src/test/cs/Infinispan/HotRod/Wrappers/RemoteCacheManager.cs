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

        public RemoteCacheManager(Configuration configuration, ISerializer serializer, bool start)
        {
            manager = new Infinispan.HotRod.RemoteCacheManager(configuration, serializer, start);
        }

        public RemoteCacheManager(Configuration configuration, ISerializer serializer)
        {
            manager = new Infinispan.HotRod.RemoteCacheManager(configuration, serializer);
        }

        public RemoteCacheManager(Configuration configuration, bool start)
        {
            // Don't serialize, use the already serialized data.
            manager = new Infinispan.HotRod.RemoteCacheManager(configuration, new IdentitySerializer(), start);
        }

        public RemoteCacheManager(Configuration configuration, bool start, bool useCompatibilityMarshaller)
        {
            if (useCompatibilityMarshaller)
                manager = new Infinispan.HotRod.RemoteCacheManager(configuration, new CompatibilitySerializer(), start);
            else
                // Don't serialize, use the already serialized data.
                manager = new Infinispan.HotRod.RemoteCacheManager(configuration, new IdentitySerializer(), start);
        }

        public RemoteCacheManager(Configuration configuration)
        {
            // Don't serialize, use the already serialized data.
            manager = new Infinispan.HotRod.RemoteCacheManager(configuration, new IdentitySerializer());
        }

        public RemoteCacheManager(bool start)
        {
            // Don't serialize, use the already serialized data.
            manager = new Infinispan.HotRod.RemoteCacheManager(new IdentitySerializer(), start);
        }

        public RemoteCacheManager()
        {
            // Don't serialize, use the already serialized data.
            manager = new Infinispan.HotRod.RemoteCacheManager(new IdentitySerializer());
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
    }
}