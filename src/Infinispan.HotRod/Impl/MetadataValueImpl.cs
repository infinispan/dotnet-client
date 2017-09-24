using Infinispan.HotRod;

namespace Infinispan.HotRod.Impl
{
#pragma warning disable 1591
    public class MetadataValueImpl<V> : VersionedValueImpl<V>, IMetadataValue<V>
    {
        private long created, lastUsed;
        private int lifespan, maxIdle;

        public MetadataValueImpl(V value, ulong version, long created, long lastUsed, int lifespan, int maxIdle) : base(value, version)
        {
            this.created = created;
            this.lastUsed = lastUsed;
            this.lifespan = lifespan;
            this.maxIdle =  maxIdle;
        }

        public long GetCreated()
        {
            return created;
        }

        public long GetLastUsed()
        {
            return lastUsed;
        }

        public int GetLifespan()
        {
            return lifespan;
        }

        public int GetMaxIdle()
        {
            return maxIdle;
        }
    }
#pragma warning restore 1591
}
