using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Util;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient.Operations;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotNetClient.Trans.Impl.TCP;
using Infinispan.DotNetClient.Impl;
using Infinispan.DotnetClient;

namespace Infinispan.DotNetClient
{
    /// <summary>
    /// Aggregates RemoteCaches and lets user to get hold of a remotecache.
    /// Author: sunimalr@gmail.com
    /// </summary>
    public class RemoteCacheManager
    {
        private ClientConfig config;
        private ISerializer serializer;
        private Codec codec;
        private TCPTransportFactory transportFactory;
        private IRequestBalancer requestBalancer;

        /// <summary>
        /// Constructor with specified serializer s
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="s"></param>
        public RemoteCacheManager(ClientConfig configuration, ISerializer s)
        {
            this.requestBalancer = new RoundRobinRequestBalancer();
            this.config = configuration;
            this.serializer = s;
            this.codec = new Codec();
            this.transportFactory = new TCPTransportFactory(this.config, this.serializer,this.requestBalancer);
        }

        /// <summary>
        /// Constructor with default serializer
        /// </summary>
        /// <param name="configuration"></param>
        public RemoteCacheManager(ClientConfig configuration)
        {
            this.requestBalancer = new RoundRobinRequestBalancer();
            this.config = configuration;
            this.serializer = new DefaultSerializer();
            this.codec = new Codec();
            this.transportFactory = new TCPTransportFactory(this.config, this.serializer, this.requestBalancer);
        }

        /// <summary>
        /// Constructor with specified serializer s
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="s"></param>
        public RemoteCacheManager(ClientConfig configuration, ISerializer s, IRequestBalancer reqBalancer)
        {
            this.requestBalancer = reqBalancer;
            this.config = configuration;
            this.serializer = s;
            this.codec = new Codec();
            this.transportFactory = new TCPTransportFactory(this.config, this.serializer, this.requestBalancer);
        }

        /// <summary>
        /// Constructor with default serializer
        /// </summary>
        /// <param name="configuration"></param>
        public RemoteCacheManager(ClientConfig configuration, IRequestBalancer reqBalancer)
        {
            this.requestBalancer = reqBalancer;
            this.config = configuration;
            this.serializer = new DefaultSerializer();
            this.codec = new Codec();
            this.transportFactory = new TCPTransportFactory(this.config, this.serializer, this.requestBalancer);
        }

        /// <summary>
        /// Cache with default settings mentioned in App.config file
        /// </summary>
        public IRemoteCache<K, V> GetCache<K,V>()
        {
            return new RemoteCacheImpl<K, V>(this, this.config,false,this.serializer, this.transportFactory);
        }

        /// <summary>
        ///Cache with default settings and a given cacheName
        /// </summary>
        public IRemoteCache<K, V> GetCache<K,V>(String cacheName)
        {
            return new RemoteCacheImpl<K, V>(this, this.config, cacheName, this.serializer, this.transportFactory);
        }

        /// <summary>
        /// Cache with specified forceRetunValue parameter
        /// </summary>
        /// <param name="forceRetunValue"></param>
        /// <returns></returns>
        public IRemoteCache<K, V> GetCache<K,V>(bool forceRetunValue)
        {
            return new RemoteCacheImpl<K, V>(this, this.config, forceRetunValue, this.serializer, this.transportFactory);
        }

        /// <summary>
        ///Specified named cache with customized forceRetunValue option
        /// </summary>
        /// <param name="cacheName">If the user needs to give the cachename manually it can be passed here</param>
        /// <param name="forceRetunValue">If forceRetunValue is true, cache returns the value existed before the operation</param>
        /// <returns></returns>
        public IRemoteCache<K, V> GetCache<K,V>(String cacheName, bool forceRetunValue)
        {
            return new RemoteCacheImpl<K, V>(this, this.config, forceRetunValue, this.serializer, this.transportFactory);
        }
    }
}
