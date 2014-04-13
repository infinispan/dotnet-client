using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinispan.HotRod.Wrappers
{
    public class VersionedValue
    {
        private Infinispan.HotRod.IVersionedValue<byte[]> value;

        internal VersionedValue(Infinispan.HotRod.IVersionedValue<byte[]> value)
        {
            this.value = value;
        }

        public byte[] GetValue()
        {
            return value.GetValue();
        }

        public long GetVersion()
        {
            return value.GetVersion();
        }
    }
}
