using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Util;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient.Trans.TCP;
using Infinispan.DotNetClient.Operations;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotnetClient;

namespace Infinispan.DotNetClient
{

    /**
     * 
     * Aggregates RemoteCaches and lets user to get hold of a remotecache.
     * Author: sunimalr@gmail.com
     * 
     */

    public class RemoteCacheManager
    {
        private ClientConfig config;
        private Serializer serializer;
        private Codec codec;
        private TCPTransportFactory transportFactory;

        public RemoteCacheManager(ClientConfig configuration, Serializer s)
        {
            this.config = configuration;
            this.serializer = s;
            this.codec = new Codec();
            this.transportFactory = new TCPTransportFactory(this.config);
        }

        public RemoteCacheManager(ClientConfig configuration)
        {
            this.config = configuration;
            this.serializer = new DefaultSerializer();
            this.codec = new Codec();
            this.transportFactory = new TCPTransportFactory(this.config);
        }

        //Cache with default settings mentioned in App.config file
        public RemoteCache getCache()
        {
            return new RemoteCacheImpl(this, this.config, this.serializer,this.transportFactory);
        }

        //Cache with default settings and a given cacheName
        public RemoteCache getCache(String cacheName)
        {
            return new RemoteCacheImpl(this, this.config, cacheName, this.serializer,this.transportFactory);
        }

        //Cache with specified forceRetunValue parameter
        public RemoteCache getCache(bool forceRetunValue)
        {
            return new RemoteCacheImpl(this, this.config, forceRetunValue, this.serializer,this.transportFactory);
        }

        //Specified named cache with customized forceRetunValue option
        public RemoteCache getCache(String cacheName, bool forceRetunValue)
        {
            return new RemoteCacheImpl(this, this.config, forceRetunValue, this.serializer, this.transportFactory);
        }
    }
}
