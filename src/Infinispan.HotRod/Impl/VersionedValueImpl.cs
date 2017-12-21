using Infinispan.HotRod;

namespace Infinispan.HotRod.Impl
{
#pragma warning disable 1591
    public class VersionedValueImpl<V> : IVersionedValue<V>
    {
        private V value;
        private ulong version;

        public VersionedValueImpl(V value, ulong version)
        {
            this.value = value;
            this.version = version;
        }
         
        public V GetValue()
        {
            return value;
        }

        public ulong GetVersion()
        {
            return this.version;
        }
    }
#pragma warning restore 1591
}
