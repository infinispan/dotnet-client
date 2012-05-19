using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinispan.DotNetClient.Protocol;
using Infinispan.DotNetClient;
using Infinispan.DotNetClient.Operations;
using Infinispan.DotNetClient.Trans;

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

        public GetOperation newGetKeyOperation(byte[] key)
        {
            return new GetOperation(codec, key, cacheNameBytes, topologyId, flags());
        }

        public RemoveOperation newRemoveOperation(byte[] key)
        {
            return new RemoveOperation(
                  codec, key, cacheNameBytes, topologyId, flags());
        }

        public RemoveIfUnmodifiedOperation newRemoveIfUnmodifiedOperation(byte[] key, long version)
        {
            return new RemoveIfUnmodifiedOperation(
                  codec, key, cacheNameBytes, topologyId, flags(), version);
        }

        public ReplaceIfUnmodifiedOperation newReplaceIfUnmodifiedOperation(byte[] key,
                 byte[] value, int lifespanSeconds, int maxIdleTimeSeconds, long version)
        {
            return new ReplaceIfUnmodifiedOperation(
                  codec, key, cacheNameBytes, topologyId, flags(),
                  value, lifespanSeconds, maxIdleTimeSeconds, version);
        }

        public GetWithVersionOperation newGetWithVersionOperation(byte[] key)
        {
            return new GetWithVersionOperation(
                  codec, key, cacheNameBytes, topologyId, flags());
        }

        public StatsOperation newStatsOperation()
        {
            return new StatsOperation(codec, cacheNameBytes, topologyId, flags());
        }

        public PutOperation newPutKeyValueOperation(byte[] key, byte[] value,
                 int lifespanSecs, int maxIdleSecs)
        {
            return new PutOperation(
                  codec, key, cacheNameBytes, topologyId, flags(),
                  value, lifespanSecs, maxIdleSecs);
        }

        public PutIFAbsentOperation newPutIfAbsentOperation(byte[] key, byte[] value,
                 int lifespanSecs, int maxIdleSecs)
        {
            return new PutIFAbsentOperation(
                  codec, key, cacheNameBytes, topologyId, flags(),
                  value, lifespanSecs, maxIdleSecs);
        }

        public ReplaceOperation newReplaceOperation(byte[] key, byte[] values,
                 int lifespanSecs, int maxIdleSecs)
        {
            return new ReplaceOperation(
                  codec, key, cacheNameBytes, topologyId, flags(),
                  values, lifespanSecs, maxIdleSecs);
        }

        public ContainsKeyOperation newContainsKeyOperation(byte[] key)
        {
            return new ContainsKeyOperation(
                  codec, key, cacheNameBytes, topologyId, flags());
        }

        public ClearOperation newClearOperation()
        {
            return new ClearOperation(
                  codec, cacheNameBytes, topologyId, flags());
        }

        public BulkGetOperation newBulkGetOperation(int size)
        {
            return new BulkGetOperation(
                  codec, cacheNameBytes, topologyId, flags(), size);
        }

        public PingOperation newPingOperation(Transport transport)
        {
            return new PingOperation(codec, topologyId, transport, cacheNameBytes);
        }

        private Flag[] flags()
        {
            //null only for basic implementation
            return null;
        }

    }
}
