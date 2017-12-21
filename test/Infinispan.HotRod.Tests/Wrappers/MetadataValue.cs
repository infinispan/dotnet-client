using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinispan.HotRod.Wrappers
{
    public class MetadataValue : VersionedValue
    {
        private Infinispan.HotRod.IMetadataValue<object> value;

        internal MetadataValue(Infinispan.HotRod.IMetadataValue<object> value) : base(value)
        {
            this.value = value;
        }

        public long GetCreated()
        {
            return value.GetCreated();
        }

        public int GetLifespan()
        {
            return value.GetLifespan();
        }

        public long GetLastUsed()
        {
            return value.GetLastUsed();
        }

        public int GetMaxIdle()
        {
            return value.GetMaxIdle();
        }
    }
}
