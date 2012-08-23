﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Operations;
using Infinispan.DotNetClient.Trans;
using Infinispan.DotnetClient;

namespace Infinispan.DotNetClient.Operations
{
    /*
     * OperationsFactory is the class that produces Operations and returns.
     * 
     * Author: sunimalr@gmail.com
     *
     */

    public class OperationsFactory : HotRodConstants
    {
        private UTF8Encoding e = new UTF8Encoding();
        private const Flag[] FORCE_RETURN_VALUE = null; //TODO
        private byte[] cacheNameBytes;
        private int topologyId;
        private bool forceReturnValue;
        private Codec codec;

        public OperationsFactory(string cacheName, int topologyId, bool forceReturnValue, Codec codec)
        {
            if (cacheName.Equals("default"))
            {
                this.cacheNameBytes = DEFAULT_CACHE_NAME_BYTES; //For base client using the default cache
            }
            else
            {
                this.cacheNameBytes = this.e.GetBytes(cacheName);
            }
            
            this.topologyId = topologyId;
            this.forceReturnValue = forceReturnValue;
            this.codec = codec;
        }

        public GetOperation NewGetKeyOperation(byte[] key)
        {
            return new GetOperation(codec, key, cacheNameBytes, topologyId, Flags(),this);
        }

        public RemoveOperation NewRemoveOperation(byte[] key)
        {
            return new RemoveOperation(
                  codec, key, cacheNameBytes, topologyId, Flags(),this);
        }

        public RemoveWithVersionOperation NewRemoveWithVersionOperation(byte[] key, long version)
        {
            return new RemoveWithVersionOperation(
                  codec, key, cacheNameBytes, topologyId, Flags(), version,this);
        }

        public ReplaceWithVersionOperation NewReplaceWithVersionOperation(byte[] key,
                 byte[] value, int lifespanSeconds, int maxIdleTimeSeconds, long version)
        {
            return new ReplaceWithVersionOperation(
                  codec, key, cacheNameBytes, topologyId, Flags(),
                  value, lifespanSeconds, maxIdleTimeSeconds, version,this);
        }

        public GetWithVersionOperation NewGetWithVersionOperation(byte[] key)
        {
            return new GetWithVersionOperation(
                  codec, key, cacheNameBytes, topologyId, Flags(),this);
        }

        public StatsOperation NewStatsOperation()
        {
            return new StatsOperation(codec, cacheNameBytes, topologyId, Flags(),this);
        }

        public PutOperation NewPutKeyValueOperation(byte[] key, byte[] value,
                 int lifespanSecs, int maxIdleSecs)
        {
            return new PutOperation(
                  codec, key, cacheNameBytes, topologyId, Flags(),
                  value, lifespanSecs, maxIdleSecs,this);
        }

        public PutIfAbsentOperation NewPutIfAbsentOperation(byte[] key, byte[] value,
                 int lifespanSecs, int maxIdleSecs)
        {
            return new PutIfAbsentOperation(
                  codec, key, cacheNameBytes, topologyId, Flags(),
                  value, lifespanSecs, maxIdleSecs,this);
        }

        public ReplaceOperation NewReplaceOperation(byte[] key, byte[] values,
                 int lifespanSecs, int maxIdleSecs)
        {
            return new ReplaceOperation(
                  codec, key, cacheNameBytes, topologyId, Flags(),
                  values, lifespanSecs, maxIdleSecs,this);
        }

        public ContainsKeyOperation NewContainsKeyOperation(byte[] key)
        {
            return new ContainsKeyOperation(
                  codec, key, cacheNameBytes, topologyId, Flags(),this);
        }

        public ClearOperation NewClearOperation()
        {
            return new ClearOperation(
                  codec, cacheNameBytes, topologyId, Flags(),this);
        }

        public BulkGetOperation NewBulkGetOperation(int size)
        {
            return new BulkGetOperation(
                  codec, cacheNameBytes, topologyId, Flags(), size,this);
        }

        public PingOperation NewPingOperation(ITransport transport)
        {
            return new PingOperation(codec, topologyId, transport, cacheNameBytes,this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Flags specified, null if no flags specified</returns>
        private Flag[] Flags()
        {
            Flag[] flags=null;
            
            if (forceReturnValue)
            {
                flags = new Flag[1];
                Flag f = new Flag(Flag.FORCE_RETURN_VALUE);
                flags[0] = f;
            }
            return flags;
        }

        public void SetTopologyId(int topId)
        {
            this.topologyId = topId;
        }

        public int GetTopologyId()
        {
            return this.topologyId;
        }
    }
}
