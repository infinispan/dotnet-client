using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.HotRod.Config;
using Infinispan.HotRod.Impl;

namespace Infinispan.HotRod
{
    /// <summary>
    ///   Factory for IRemoteCache instances.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     <b>Lifecycle:</b> In order to be able to use an IRemoteCache, the RemoteCacheManager must be started first:
    /// beside other things, this instantiates connections to Hot Rod server(s). Starting the RemoteCacheManager can be
    /// done either at creation by passing start==true to constructor or by using a constructor that does that for you
    /// (see C-tor documentation); or after construction by calling Start().
    ///   </para>
    ///   <para>
    ///     This is an "expensive" object, as it manages a set of persistent TCP connections to the Hot Rod servers.
    /// It is recommended to only have one instance of this per process, and to cache it between calls to the server
    /// (i.e. remoteCache operations).
    ///   </para>
    ///   <para>
    ///     Stop() needs to be called explicitly in order to release all the resources (e.g. threads, TCP connections).
    ///   </para>
    /// </remarks>
    public class RemoteCacheManager
    {
        private Infinispan.HotRod.SWIG.RemoteCacheManager manager;
        private IMarshaller marshaller;
        private IMarshaller argMarshaller;
        private Configuration configuration;
        /// <summary>
        /// Construct an instance with default configuration and marshaller.
        /// </summary>
        /// <param name="start"></param>
        public RemoteCacheManager(bool start = true)
        {
            this.marshaller = new DefaultMarshaller();
            if (Infinispan.HotRod.SWIG.Util.Use64())
            {
                manager = new Infinispan.HotRod.SWIGGen.RemoteCacheManager(start);
            }
            else {
                manager = new Infinispan.HotRod.SWIGGen.RemoteCacheManager(start);
            }
        }

        /// <summary>
        /// Construct an instance with specific configuration.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="start"></param>
        public RemoteCacheManager(Configuration configuration, bool start = true)
        {
            this.marshaller = configuration.Marshaller();
            this.configuration = configuration;
            if (Infinispan.HotRod.SWIG.Util.Use64())
            {
                manager = new Infinispan.HotRod.SWIGGen.RemoteCacheManager((Infinispan.HotRod.SWIGGen.Configuration)configuration.Config(), start);
            }
            else {
                manager = new Infinispan.HotRod.SWIGGen.RemoteCacheManager((Infinispan.HotRod.SWIGGen.Configuration)configuration.Config(), start);
            }
        }

        /// <summary>
        /// Construct an instance with specific configuration and serializer.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="marshaller"></param>
        /// <param name="argMarshaller"></param>
        /// <param name="start"></param>
        public RemoteCacheManager(Configuration configuration, IMarshaller marshaller, bool start = true, IMarshaller argMarshaller = null)
        {
            this.marshaller = marshaller;
            this.argMarshaller = argMarshaller;
            this.configuration = configuration;

            if (Infinispan.HotRod.SWIG.Util.Use64())
            {
                manager = new Infinispan.HotRod.SWIGGen.RemoteCacheManager((Infinispan.HotRod.SWIGGen.Configuration)configuration.Config(), start);
            }
            else
            {
                manager = new Infinispan.HotRod.SWIGGen.RemoteCacheManager((Infinispan.HotRod.SWIGGen.Configuration)configuration.Config(), start);
            }
        }

        /// <summary>
        /// Construct an instance with default configuration and specific serializer.
        /// </summary>
        /// <param name="marshaller"></param>
        /// <param name="argMarshaller"></param>
        /// <param name="start"></param>
        public RemoteCacheManager(IMarshaller marshaller, bool start = true, IMarshaller argMarshaller = null)
        {
            this.marshaller = marshaller;
            this.argMarshaller = argMarshaller;
            if (Infinispan.HotRod.SWIG.Util.Use64())
            {
                manager = new Infinispan.HotRod.SWIGGen.RemoteCacheManager(start);
            }
            else
            {
                manager = new Infinispan.HotRod.SWIGGen.RemoteCacheManager(start);
            }
        }

        /// <summary>
        ///   Starts the manager.
        /// </summary>
        public void Start()
        {
            manager.Start();
        }

        /// <summary>
        ///   Stops the manager.
        /// </summary>
        public void Stop()
        {
            manager.Stop();
        }

        /// <summary>
        ///   Can be used to check if the manager is started or not.
        /// </summary>
        ///
        /// <returns>true if the cache manager is started and false otherwise</returns>
        public bool IsStarted()
        {
            return manager.IsStarted();
        }

        /// <summary>
        ///   Retrieves the default cache from the remote server.
        /// </summary>
        /// <param name="m">the marshaller policy for this cache</param>
        ///
        /// <returns>a remote cache instance which can be used to send requests to the default cache</returns>
        public IRemoteCache<K, V> GetCache<K, V>(IMarshaller m = null)
        {
            if (Infinispan.HotRod.SWIG.Util.Use64())
            {
                return new RemoteCacheSWIGGenImpl<K, V>(manager, manager.GetByteArrayCache(), (m != null) ? m : marshaller, argMarshaller, configuration);
            }
            else {
                return new RemoteCacheSWIGGenImpl<K, V>(manager, manager.GetByteArrayCache(), (m != null) ? m : marshaller, argMarshaller, configuration);
            }
        }

        /// <summary>
        ///   Retrieves a named cache from the remote server. If the cache has been previously created with the same
        ///   name, the running cache instance is returned.  Otherwise, this method attempts to create the cache first.
        /// </summary>
        ///
        /// <param name="cacheName">the name of the cache</param>
        /// <param name="m">the marshaller policy for this cache</param>
        ///
        /// <returns>a remote cache instance which can be used to send requests to the named cache</returns>
        public IRemoteCache<K, V> GetCache<K, V>(String cacheName, IMarshaller m = null)
        {
            if (Infinispan.HotRod.SWIG.Util.Use64())
            {
                return new RemoteCacheSWIGGenImpl<K, V>(manager, manager.GetByteArrayCache(cacheName), (m != null) ? m : marshaller, argMarshaller, configuration);
            }
            else {
                return new RemoteCacheSWIGGenImpl<K, V>(manager, manager.GetByteArrayCache(cacheName), (m != null) ? m : marshaller, argMarshaller, configuration);
            }
        }

        /// <summary>
        ///   Retrieves the default cache from the remote server.
        /// </summary>
        ///
        /// <param name="forceReturnValue">indicates if the force return value flag should be enabled or not</param>
        /// <param name="m">the marshaller policy for this cache</param>
        ///
        /// <returns>a remote cache instance which can be used to send requests to the default cache</returns>
        public IRemoteCache<K, V> GetCache<K, V>(bool forceReturnValue, IMarshaller m = null)
        {
            if (Infinispan.HotRod.SWIG.Util.Use64())
            {
                return new RemoteCacheSWIGGenImpl<K, V>(manager, manager.GetByteArrayCache(forceReturnValue), (m != null) ? m : marshaller, argMarshaller, configuration);
            }
            else {
                return new RemoteCacheSWIGGenImpl<K, V>(manager, manager.GetByteArrayCache(forceReturnValue), (m != null) ? m : marshaller,  argMarshaller,configuration);
            }
        }

        /// <summary>
        ///   Retrieves a named cache from the remote server. If the cache has been previously created with the same
        ///   name, the running cache instance is returned.  Otherwise, this method attempts to create the cache first.
        /// </summary>
        ///
        /// <param name="cacheName">the name of the cache</param>
        /// <param name="forceReturnValue">indicates if the force return value flag should be enabled or not</param>
        /// <param name="m">the marshaller policy for this cache</param>
        ///
        /// <returns>a remote cache instance which can be used to send requests to the named cache</returns>
        public IRemoteCache<K, V> GetCache<K, V>(String cacheName, bool forceReturnValue, IMarshaller m = null)
        {
            if (Infinispan.HotRod.SWIG.Util.Use64())
            {
                return new RemoteCacheSWIGGenImpl<K, V>(manager, manager.GetByteArrayCache(cacheName, forceReturnValue), (m != null) ? m : marshaller, argMarshaller, configuration);
            }
            else {
                return new RemoteCacheSWIGGenImpl<K, V>(manager, manager.GetByteArrayCache(cacheName, forceReturnValue), (m != null) ? m : marshaller, argMarshaller, configuration);
            }
        }

        /// <summary>
        /// Perform a cluster switch to the cluster with name clusterName
        /// </summary>
        /// <param name="clusterName"></param>
        /// <returns>true if the switch successed, false otherwise</returns>
        public bool SwitchToCluster(string clusterName)
        {
            return manager.SwitchToCluster(clusterName);
        }

        /// <summary>
        /// Perform a cluster switch to the default cluster (the one defined by ConfigurationBuilder.addServer() method)
        /// </summary>
        /// <returns>true if the switch successed, false otherwise</returns>
        public bool SwitchToDefaultCluster()
        {
            return manager.SwitchToDefaultCluster();
        }
    }
}
