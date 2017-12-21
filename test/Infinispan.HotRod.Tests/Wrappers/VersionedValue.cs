using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinispan.HotRod.Wrappers
{
    public class VersionedValue
    {
        private Infinispan.HotRod.IVersionedValue<object> value;

        internal VersionedValue(Infinispan.HotRod.IVersionedValue<object> value)
        {
            this.value = value;
        }

        public object GetValue()
        {
            return value.GetValue();
        }

        public ulong GetVersion()
        {
            return value.GetVersion();
        }
    }
}
